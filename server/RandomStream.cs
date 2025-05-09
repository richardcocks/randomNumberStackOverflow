namespace RandomNumberCore;

sealed class RandomStream: Stream
{
    public RandomStream( Random random, CancellationToken token )
    {
        this.random = random;
        cancelReg = token.Register( Dispose );
    }

    readonly Random random;
    long sequence = 0;
    CancellationTokenRegistration? cancelReg;
    volatile bool isDisposed = false;

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => sequence;
        set => throw new NotSupportedException();
    }

    public override void Flush() { }

    public override int Read( byte[] buffer, int offset, int count )
    {
        if( isDisposed )
            throw new ObjectDisposedException( null );

        var internalBuffer = new Span<byte>( buffer, offset, count );
        random.NextBytes( internalBuffer );
        sequence += count;
        return count;
    }

    public override int Read( Span<byte> buffer )
    {
        if( isDisposed )
            throw new ObjectDisposedException( null );

        random.NextBytes( buffer );
        sequence += buffer.Length;
        return buffer.Length;
    }

    public override long Seek( long offset, SeekOrigin origin ) =>
        throw new NotSupportedException();

    public override void SetLength( long value ) =>
        throw new NotSupportedException();

    public override void Write( byte[] buffer, int offset, int count ) =>
        throw new NotSupportedException();

    protected override void Dispose( bool disposing )
    {
        base.Dispose( disposing );
        isDisposed = true;
        cancelReg?.Unregister();
        cancelReg = null;
    }
}
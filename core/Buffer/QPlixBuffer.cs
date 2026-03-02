namespace core.Buffer;

class QPlixBuffer : Dictionary<BufferKey, double>
{
    
}

class BufferKey
{
    public string Name { get; set; } = default!;
    public DateOnly Date { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is not BufferKey other)
            return false;

        return other.Name == Name && other.Date == Date;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Date);
    }
}

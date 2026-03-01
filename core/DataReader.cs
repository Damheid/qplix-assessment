using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace core;

internal class DataReader : IDisposable
{
    private readonly StreamReader streamReader;
    private readonly CsvReader csvReader;

    public DataReader(string filePath)
    {
        streamReader = new StreamReader(filePath);
        csvReader = GetReader();
    }

    private CsvReader GetReader()
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";",
        };

        return new CsvReader(streamReader, config);
    }
    public List<T> ReadCsv<T, TMapper>() where TMapper : ClassMap
    {
        csvReader.Context.RegisterClassMap<TMapper>();
        var records = csvReader.GetRecords<T>().ToList();

        return records;
    }

    public List<T> ReadCsv<T>()
    {
        var records = csvReader.GetRecords<T>().ToList();

        return records;
    }

    public void Dispose()
    {
        csvReader.Dispose();
        streamReader.Dispose();

        GC.SuppressFinalize(this);
    }
}
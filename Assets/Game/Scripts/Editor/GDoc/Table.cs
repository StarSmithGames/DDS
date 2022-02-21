using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;

namespace Game.Editor.GDocs
{
    public class Table
    {
        public delegate void OnRowProcess(IList<CellEntry> entry);

        private TableData data;
        private SheetsService service;

        public Table(TableData data, string keyPath)
        {
            this.data = data;

            GoogleCredential googleCredential;
            using (Stream stream = new FileStream(keyPath, FileMode.Open, FileAccess.Read))
            {
                googleCredential = GoogleCredential.FromStream(stream).CreateScoped(SheetsService.Scope.Spreadsheets);

            }
            // Create Google Sheets API service.
            service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = googleCredential,
            });
        }

        public IList<IList<object>> GetSheet(string sheet)
		{
            return GetSheetRange(sheet);
        }

        private IList<IList<object>> GetSheetRange(string range)
        {
            var request = service.Spreadsheets.Values.Get(data.id, range);
            IList<IList<object>> table = request.Execute().Values;

            if (table == null)
            {
                throw new Exception("Table == null");
            }

            return table;
        }

        public List<string> GetHeaders(string sheet)
		{
            var values = GetSheet(sheet);
            return GetHeaders(values);
        }
        public List<string> GetHeaders(IList<IList<object>> values)
		{
            return values.First().Select(x => (x as string).TrimStart('#')).ToList();
        }

        public void Map(string sheet, OnRowProcess callback)
        {
            try
            {
                var values = GetSheet(sheet);

                if (values != null && values.Count > 0)
                {
                    // Process first row as headers
                    var headers = GetHeaders(values);

                    foreach (var row in values.Skip(1))
                    {
                        var entryRow = row.Select((item, index) => index < headers.Count
                            ? new CellEntry { LocalName = headers[index], Value = item as string }
                            : new CellEntry { LocalName = string.Empty, Value = item as string }).ToList();
                   
                        callback(entryRow);
                    }
                }
            }
            catch (Exception e)
            {
               throw new Exception("Can't import: " + sheet);
            }
        }

        public string GetElementValue(IList<CellEntry> row, string key)
        {
            var entry = GetElement(row, key);
            if (entry == null)
            {
                return "";
            }

            return entry.Value == null ? "" : entry.Value.Trim();
        }

        public CellEntry GetElement(IList<CellEntry> row, string key)
        {
            foreach (var el in row)
            {
                if (el.LocalName == key)
                {
                    return el;
                }
            }

            return null;
        }

        public class CellEntry
        {
            public string LocalName;
            public string Value;
        }
    }
}
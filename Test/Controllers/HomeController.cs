using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Test.DataBas_MSSQL.Context;
using Test.Models;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Microsoft.AspNetCore.Http;
using System.IO;
using NPOI.SS.Formula.Functions;
using Microsoft.EntityFrameworkCore;

namespace Test.Controllers;

public class HomeController(DataBaseContext context) : Controller
{
    private DataBaseContext _context = context;
    private IWorkbook? _workbook;
    private List<WeatherData>? _weatherData;

    public IActionResult HomePage()
    {
        return View();
    }

    public async Task<IActionResult> Archives()
    {
        _weatherData = await _context.WeatherData.ToListAsync();
        ViewBag.Years = await _context.WeatherData
            .Select(x => x.Date.Value.Year)
            .Distinct()
            .OrderByDescending(x => x)
            .ToListAsync();

        return View("Archives", _weatherData);
    }

    public async Task<IActionResult> FilterData(int? month, int? year)
    {
        IQueryable<WeatherData> query = _context.WeatherData;

        if (month.HasValue)
        {
            query = query.Where(x => x.Date.Value.Month == month.Value);
        }

        if (year.HasValue)
        {
            query = query.Where(w => w.Date.Value.Year == year.Value);
        }

        var weatherData = await query.ToListAsync();

        ViewBag.Years = await _context.WeatherData
            .Select(x => x.Date.Value.Year)
            .Distinct()
            .OrderByDescending(x => x)
            .ToListAsync();

        return View("Archives", weatherData);
    }

    [HttpPost]
    public async Task<IActionResult> LoadData([FromForm] List<IFormFile> weatherFiles)
    {
        if (weatherFiles == null || weatherFiles.Count == 0)
        {
            ViewBag.Message = "Пожалуйста, выберите файлы.";
            return View("HomePage");
        }

        foreach (var weatherFile in weatherFiles)
        {
            if (weatherFile == null || weatherFile.Length == 0)
            {
                ViewBag.Message = "Пожалуйста, выберите файл.";
                return View("HomePage");
            }

            string fileExtension = Path.GetExtension(weatherFile.FileName).ToLower();

            if (fileExtension != ".xlsx")
            {
                ViewBag.Message = "Пожалуйста, загрузите Excel файл (.xlsx).";
                return View("HomePage");
            }

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await weatherFile.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    _workbook = new XSSFWorkbook(memoryStream);

                    for (int s = 0; s < _workbook.NumberOfSheets; s++)
                    {
                        ISheet sheet = _workbook.GetSheetAt(s);

                        for (int r = 4; r < 9999; r++)
                        {
                            IRow row = sheet.GetRow(r);
                            if (row == null) continue;

                            var date = GetDateCellValue(row, 0);
                            var timeAsDateTime = GetDateCellValue(row, 1).Value.TimeOfDay;
                            var temperature = GetNumericCellValue(row, 2);
                            var relativeHumidity = GetNumericCellValue(row, 3);
                            var dewPoint = GetNumericCellValue(row, 4);
                            var atmosphericPressure = GetNumericCellValue(row, 5);
                            string? windDirection = row.GetCell(6)?.StringCellValue?.Trim();
                            var windSpeed = GetNumericCellValue(row, 7);
                            var cloudiness = GetNumericCellValue(row, 8);
                            var cloudHeight = GetNumericCellValue(row, 9);
                            var visibility = GetNumericCellValue(row, 10);
                            string? weatherPhenomena = row.GetCell(11)?.StringCellValue?.Trim();

                            await _context.AddAsync(
                                new WeatherData
                                {
                                    Date = date,
                                    Time = timeAsDateTime,
                                    Temperature = temperature,
                                    RelativeHumidity = (int?)relativeHumidity,
                                    DewPoint = dewPoint,
                                    AtmosphericPressure = atmosphericPressure,
                                    WindDirection = windDirection,
                                    WindSpeed = windSpeed,
                                    Cloudiness = (int?)cloudiness,
                                    CloudHeight = (int?)cloudHeight,
                                    Visibility = visibility,
                                    WeatherPhenomena = weatherPhenomena
                                }
                            );

                        }
                    }
                }
                await _context.SaveChangesAsync();
            }
            catch
            {
                ViewBag.Message = "Ошибка в чтении файла!";
                return View("HomePage");
            }
        }
        ViewBag.Message = "Данные успешно загружены";
        return View("HomePage");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    #region Methods
    private DateTime? GetDateCellValue(IRow row, int cellIndex)
    {
        ICell cell = row.GetCell(cellIndex);
        if (cell == null) return null;

        if (cell.CellType == CellType.Formula || cell.CellType == CellType.String)
        {
            try
            {
                if (DateTime.TryParse(cell.StringCellValue, out DateTime res))
                {
                    return res;
                }
            }
            catch
            {
                return null;
            }
        }
        else if(cell.CellType == CellType.Numeric)
        {
            try
            {
                return cell.DateCellValue;
            }
            catch
            {
                return null;
            }
        }
        return null;
    }

    private double? GetNumericCellValue(IRow row, int cellIndex)
    {
        ICell cell = row.GetCell(cellIndex);
        if (cell == null) return null;

        if (cell.CellType == CellType.Numeric || cell.CellType == CellType.Formula)
        {
            return cell.NumericCellValue;
        }

        if (cell.CellType == CellType.String) 
        {
            if (double.TryParse(cell.StringCellValue, out double parsedValue))
            {
                return parsedValue;
            }
        }
        return null;
    }
    #endregion
}

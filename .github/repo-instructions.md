# Repository Instructions for BsdFinalProject

## מה זה הפרויקט הזה

API ב-ASP.NET Core 9.0 עבור מערכת ניהול פרויקט בידיים. יש ממשק יחיד הנמצא ב-`BsdFinalProject/`.
הפרויקט כולל ממשק רישום/התחברות, ניהול מתנות, קטגוריות, סל קניות, כרטיסים ותורמים.

---

## מבנה הפתרון

```
BsdFinalProject.sln
└── BsdFinalProject/
    ├── Controllers/      → שליטה ב-API endpoints
    ├── Data/             → `SaleContext` ו-factory עבור EF Core
    ├── DTOs/             → תעבורה בין ממשק המשתמש לשרת
    ├── IRepositories/    → ממשקי Repository
    ├── IServices/        → ממשקי שירותי עסק
    ├── Middleware/       → middleware מותאם אישית
    ├── Models/           → מודלי דומיין ו-Entities
    ├── Repositories/     → מימושי Repository
    ├── Services/         → מימושי עסק ולוגיקה
    ├── Program.cs        → רישום DI, תצורת JWT, CORS ומידלוור
    ├── appsettings.json  → חיבור למסד הנתונים והגדרות JWT
    ├── migrations/      → בסיסי נתונים EF Core
    └── logs/             → קבצי לוג של Serilog
```

### קבצים מרכזיים

- `BsdFinalProject/Data/SaleContext.cs` — DbContext עם כל DbSet ותצורת EF Core.
- `BsdFinalProject/Program.cs` — רישום שירותים, DI והגדרת צינור הבקשה.
- `BsdFinalProject/Repositories/` — מחלקות גישה לנתונים ששואבות מה-DbContext.
- `BsdFinalProject/Services/` — לוגיקת העסק ופעולות חוצות מחלקות.
- `BsdFinalProject/DTOs/` — קבצי DTO לתקשורת API, אין לחשוף `Models` ישירות.

---

## טכנולוגיות

- **Framework:** ASP.NET Core 9.0
- **ORM:** Entity Framework Core עם SQL Server
- **Authentication:** JWT Bearer
- **Logging:** Serilog (`logs/log.txt`)
- **Swagger:** נרשם ב-`Program.cs`
- **CORS:** מדיניות `AllowAll` מוגדרת, כרגע פתוחה לצורך דיבוג

---

## איך להוסיף תכונה חדשה (סדר מדויק)

1. **הוספת מודל**
   - הוסף קובץ ב-`BsdFinalProject/Models/` עבור ה-Entity.
   - הוסף `DbSet<MyEntity>` ל-`SaleContext` והגדר אותו ב-`OnModelCreating` במקרה הצורך.
2. **הוספת DTOs**
   - הוסף `record` חדש ב-`BsdFinalProject/DTOs/`, למשל `MyEntityDto`, `CreateMyEntityDto`.
3. **הוספת ממשק Repository**
   - הוסף קובץ ב-`BsdFinalProject/IRepositories/` עם `I{Entity}Repository`.
4. **הוספת מחלקת Repository**
   - מימוש ב-`BsdFinalProject/Repositories/` שמזריק `SaleContext`.
5. **הוספת ממשק Service**
   - הוסף קובץ ב-`BsdFinalProject/IServices/` עם `I{Entity}Service`.
6. **הוספת מחלקת Service**
   - מימוש ב-`BsdFinalProject/Services/` שמזריק `I{Entity}Repository`.
7. **רישום DI**
   - הוסף ב-`BsdFinalProject/Program.cs` את `.AddScoped<I{Entity}Repository, {Entity}Repository>()` ו-`.AddScoped<I{Entity}Service, {Entity}Service>()`.
8. **הוספת Controller**
   - הוסף קובץ ב-`BsdFinalProject/Controllers/` עם `[Route("api/[controller]")]`.

---

## כללים מחמירים

### שמות
- Entity: שם יחיד ב-PascalCase, למשל `Gift`, `User`, `Basket`.
- DTO: `*Dto`, `*CreateDto`, `*UpdateDto`.
- Repository: `I{Entity}Repository` / `{Entity}Repository`.
- Service: `I{Entity}Service` / `{Entity}Service`.
- Controller route: `api/{name}` בקטן.

### DTOs
- DTOs צריכים להיות `record` ב-C#.
- יש להשתמש ב-DTOs לתגובת API; לא להחזיר ישירות `Models`.

### דפוס Controller

```csharp
[HttpGet("{id}")]
public async Task<ActionResult<MyDto>> GetById(int id)
{
    var result = await _service.GetById(id);
    if (result == null) return NotFound();
    return Ok(result);
}

[HttpPost]
public async Task<ActionResult<MyDto>> Create(CreateMyDto dto)
{
    try
    {
        var result = await _service.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
    catch (Exception ex)
    {
        return BadRequest(new { message = ex.Message });
    }
}
```

- השתמש תמיד ב-`ILogger<T>`.
- החזר `NotFound()` כאשר מקור לא נמצא.
- החזר `NoContent()` לעדכונים ומחיקות מוצלחות.
- אל תחזיר ישירות Entities מתוך Controller.

### דפוס Service
- כל המתודות צריכות להיות `async Task<T>`.
- למחוק חריגות עסקיות, הטל `Exception` עם הודעה ברורה.
- אם משאב לא נמצא, החזר `null` והנח ל-Controller להחזיר 404.

### דפוס Repository
- הזרק `SaleContext` ישירות.
- השתמש ב-`FirstOrDefaultAsync`, `ToListAsync`, `FindAsync` לפי הצורך.
- כלול `Include()` כאשר ישנו יחסי גומלין שצריכים טעינה מפורשת.

---

## כללים ספציפיים לפרויקט

- אין לשנות את `SaleContextFactory` או את המבנה של `Program.cs` ללא צורך ברור.
- אם יש שימוש ב-JWT, בדוק את `Jwt:Key`, `Jwt:Issuer`, ו-`Jwt:Audience` ב-`appsettings.json`.
- שמור על ההיררכיה של `Repositories/` ו-`Services/` נפרדת.
- הימנע מעבודה על Controller ו-Repository בו זמנית אם לא נדרש.

---

## בדיקות

- השתמש ב-PowerShell או ב-`findstr` במקום `grep`/`tail`.
- אם מתווספת ספריית בדיקות, הרץ `dotnet test` עבור הפרויקט המתאים.

---

## בנייה והרצה

```powershell
dotnet restore
dotnet build
dotnet run --project BsdFinalProject
```

---

## דבר אחד חשוב

המסמך הזה נועד להסביר את תבנית העבודה בפרויקט הנוכחי. בתשומות חדשניות, תמיד בדוק תחילה את `Program.cs`, את מבנה ה-DbContext ב-`Data/`, ואת מבנה ה-DTOs.

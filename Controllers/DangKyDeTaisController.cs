
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Models;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Data;

public class DangKyDeTaisController : Controller
{
    private readonly AppDbContext _context;

    public DangKyDeTaisController(AppDbContext context)
    {
        _context = context;
    }

    // GET: DANGKYDETAIS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.DangKyDeTais.ToListAsync());
    }

    // GET: DANGKYDETAIS/Details/5
    public async Task<IActionResult> Details(int? madangky)
    {
        if (madangky == null)
        {
            return NotFound();
        }

        var dangkydetai = await _context.DangKyDeTais
            .FirstOrDefaultAsync(m => m.MaDangKy == madangky);
        if (dangkydetai == null)
        {
            return NotFound();
        }

        return View(dangkydetai);
    }

    // GET: DANGKYDETAIS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: DANGKYDETAIS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("MaDangKy,MaSinhVien,MaDeTai,NgayDangKy,LyDoDangKy,TrangThai,GhiChuDuyet,SinhVien,DeTai,PhanCongHuongDans")] DangKyDeTai dangkydetai)
    {
        if (ModelState.IsValid)
        {
            _context.Add(dangkydetai);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(dangkydetai);
    }

    // GET: DANGKYDETAIS/Edit/5
    public async Task<IActionResult> Edit(int? madangky)
    {
        if (madangky == null)
        {
            return NotFound();
        }

        var dangkydetai = await _context.DangKyDeTais.FindAsync(madangky);
        if (dangkydetai == null)
        {
            return NotFound();
        }
        return View(dangkydetai);
    }

    // POST: DANGKYDETAIS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? madangky, [Bind("MaDangKy,MaSinhVien,MaDeTai,NgayDangKy,LyDoDangKy,TrangThai,GhiChuDuyet,SinhVien,DeTai,PhanCongHuongDans")] DangKyDeTai dangkydetai)
    {
        if (madangky != dangkydetai.MaDangKy)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(dangkydetai);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DangKyDeTaiExists(dangkydetai.MaDangKy))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(dangkydetai);
    }

    // GET: DANGKYDETAIS/Delete/5
    public async Task<IActionResult> Delete(int? madangky)
    {
        if (madangky == null)
        {
            return NotFound();
        }

        var dangkydetai = await _context.DangKyDeTais
            .FirstOrDefaultAsync(m => m.MaDangKy == madangky);
        if (dangkydetai == null)
        {
            return NotFound();
        }

        return View(dangkydetai);
    }

    // POST: DANGKYDETAIS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? madangky)
    {
        var dangkydetai = await _context.DangKyDeTais.FindAsync(madangky);
        if (dangkydetai != null)
        {
            _context.DangKyDeTais.Remove(dangkydetai);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool DangKyDeTaiExists(int? madangky)
    {
        return _context.DangKyDeTais.Any(e => e.MaDangKy == madangky);
    }
}

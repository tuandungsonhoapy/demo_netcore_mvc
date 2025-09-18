using demo_netcore_mvc.Models;
using demo_netcore_mvc.ObjectData;
using Microsoft.EntityFrameworkCore;

namespace demo_netcore_mvc.AppDBContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSet cho từng bảng
        public DbSet<Khoa> Khoa { get; set; }
        public DbSet<GiangVien> GiangVien { get; set; }
        public DbSet<SinhVien> SinhVien { get; set; }
        public DbSet<DeTai> DeTai { get; set; }
        public DbSet<HuongDan> HuongDan { get; set; }
        public DbSet<Account> Account { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Khoa
            modelBuilder.Entity<Khoa>()
                .HasKey(k => k.MaKhoa);

            // GiangVien
            modelBuilder.Entity<GiangVien>()
                .HasKey(gv => gv.MaGV);

            modelBuilder.Entity<GiangVien>()
                .HasOne(gv => gv.Khoa)
                .WithMany(k => k.GiangViens)
                .HasForeignKey(gv => gv.MaKhoa);

            modelBuilder.Entity<GiangVien>()
                .HasOne(gv => gv.Account)
                .WithOne(a => a.GiangVien)
                .HasForeignKey<GiangVien>(gv => gv.AccountId);

            // SinhVien
            modelBuilder.Entity<SinhVien>()
                .HasKey(sv => sv.MaSV);

            modelBuilder.Entity<SinhVien>()
                .HasOne(sv => sv.Khoa)
                .WithMany(k => k.SinhViens)
                .HasForeignKey(sv => sv.MaKhoa);

            modelBuilder.Entity<SinhVien>()
                .HasOne(sv => sv.Account)
                .WithOne(a => a.SinhVien)
                .HasForeignKey<SinhVien>(sv => sv.AccountId);

            // DeTai
            modelBuilder.Entity<DeTai>()
                .HasKey(dt => dt.MaDT);

            modelBuilder.Entity<DeTai>()
                .HasOne(dt => dt.GiangVien)
                .WithMany(gv => gv.DeTais)
                .HasForeignKey(dt => dt.NguoiHuongDan);

            modelBuilder.Entity<DeTai>()
                .HasOne(dt => dt.Khoa)
                .WithMany(k => k.DeTais)
                .HasForeignKey(dt => dt.MaKhoa);

            // HuongDan
            modelBuilder.Entity<HuongDan>()
                .HasKey(hd => new { hd.MaSV, hd.MaDT });

            modelBuilder.Entity<HuongDan>()
                .HasOne(hd => hd.SinhVien)
                .WithMany(sv => sv.HuongDans)
                .HasForeignKey(hd => hd.MaSV);

            modelBuilder.Entity<HuongDan>()
                .HasOne(hd => hd.DeTai)
                .WithMany(dt => dt.HuongDans)
                .HasForeignKey(hd => hd.MaDT);

            modelBuilder.Entity<HuongDan>()
                .HasOne(hd => hd.GiangVien)
                .WithMany(gv => gv.HuongDans)
                .HasForeignKey(hd => hd.MaGV);

            // Cấu hình cho các view không có khóa chính
            modelBuilder.Entity<GiangVienData>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<GiangVienDetailData>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<SinhVienData>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<AccountData>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<DeTaiData>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<DeTaiDetailData>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<NamHocData>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<SinhVien_DeTai_HK_Data>(entity =>
            {
                entity.HasNoKey();
            });
        }
    }
}

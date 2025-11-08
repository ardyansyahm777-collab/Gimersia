# Cara memeriksa apakah repo ini terhubung ke GitHub

1. Buka terminal di folder proyek:
   - `cd c:\Project\Gimersia`

2. Periksa remote:
   - `git remote -v`
   - Jika output menampilkan URL yang mengandung `github.com`, repo sudah terhubung.

3. Jika belum ada remote, hubungkan ke GitHub:
   - Buat repository baru di GitHub (mis. `USERNAME/REPO`).
   - Jalankan:
     - `git remote add origin https://github.com/USERNAME/REPO.git`
     - `git branch -M main`  # atau `master` sesuai pengaturan
     - `git push -u origin main`

4. Alternatif: gunakan GitHub CLI (jika terpasang):
   - `gh repo create USERNAME/REPO --public --source=. --remote=origin --push`

Skrip tambahan ada di `scripts/check-remote.ps1` untuk memeriksa otomatis.

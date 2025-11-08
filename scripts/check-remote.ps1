# Auto detect text files and perform LF normalization
* text=auto

# Cek remote git dan konfirmasi apakah mengarah ke GitHub
# Usage: jalankan dari folder repository: .\scripts\check-remote.ps1

try {
    $remotes = git remote -v 2>$null
    if (-not $remotes) {
        Write-Output "Tidak ditemukan remote git. Jalankan 'git remote -v' atau tambahkan remote ke GitHub."
        exit 0
    }
    Write-Output "Daftar remote:"
    Write-Output $remotes
    if ($remotes -match "github\.com") {
        Write-Output "Status: Terhubung ke GitHub (ada URL yang mengandung github.com)."
    } else {
        Write-Output "Status: Tidak terhubung ke GitHub (tidak ada URL github.com)."
    }
} catch {
    Write-Output "Terjadi kesalahan saat memeriksa remote git. Pastikan git terinstal dan Anda berada di folder repo."
}
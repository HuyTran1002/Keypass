# Keypass - Quick Start Guide

## 🚀 Bắt đầu nhanh

### Cài đặt & Chạy (1 phút)

**Bước 1: Tải ứng dụng**
- Download file `Keypass.exe` từ thư mục `publish/`
- Hoặc build từ source code (xem BUILD_GUIDE.md)

**Bước 2: Chạy ứng dụng**
- Nhấp đúp vào `Keypass.exe`
- Ứng dụng sẽ hiện icon trên thanh tray (dưới cùng bên phải)

**Bước 3: Hoàn tất!**
- Ứng dụng sẵn sàng sử dụng

---

## 📖 Sử dụng cơ bản (5 phút)

### Lần đầu sử dụng

1. **Truy cập website/ứng dụng yêu cầu đăng nhập**
   - Ví dụ: Gmail, Facebook, Zalo, v.v.

2. **Nhập tài khoản & mật khẩu**
   - Username: `example@gmail.com`
   - Password: `password123`

3. **Bấm Enter hoặc click nút Đăng nhập**
   - Ứng dụng sẽ phát hiện

4. **Popup xuất hiện hỏi lưu mật khẩu**
   - Click "Có" để lưu
   - Click "Không" để bỏ qua

5. **Hoàn tất!**
   - Tài khoản đã được lưu

---

## 🔐 Sử dụng Auto-Fill

### Lần tới khi đăng nhập cùng tài khoản

1. **Truy cập vào trang đăng nhập**
   - Cùng website hoặc ứng dụng

2. **Click vào ô Username**
   - Một popup sẽ hiện ra

3. **Chọn tài khoản từ popup**
   - Thấy danh sách tài khoản đã lưu
   - Click chọn cái bạn muốn

4. **Tự động điền**
   - Username được điền vào tự động
   - Password được điền vào tự động

5. **Bấm Enter hoặc click Đăng nhập**
   - Xong! Bạn đã đăng nhập

---

## 📋 Quản lý Tài khoản

### Mở Trình quản lý mật khẩu

**Cách 1**: Nhấp đúp vào icon Keypass trên tray

**Cách 2**: Nhấp chuột phải → Manage Passwords

### Thêm tài khoản thủ công

1. Mở Trình quản lý
2. Click nút "Add"
3. Nhập:
   - Website: `gmail.com`
   - Username: `your.email@gmail.com`
   - Password: `your_password`
4. Click "Save"

### Chỉnh sửa tài khoản

1. Tìm tài khoản cần chỉnh sửa
2. Click để chọn
3. Click nút "Edit"
4. Thay đổi thông tin
5. Click "Save"

### Xóa tài khoản

1. Tìm tài khoản cần xóa
2. Click để chọn
3. Click nút "Delete"
4. Xác nhận xóa

### Tìm kiếm

- Sử dụng ô "Search" ở phía trên
- Gõ tên website để tìm
- Kết quả sẽ hiện ngay

---

## ⚙️ Cài đặt

### Truy cập Settings

1. Nhấp chuột phải vào icon Keypass
2. Chọn "Settings"

### Tùy chọn cài đặt

**Enable Auto-Fill**
- ✓ Bật: Gợi ý tài khoản khi click vào ô nhập
- ☐ Tắt: Không gợi ý

**Ask to save new credentials**
- ✓ Bật: Hỏi mỗi lần có tài khoản mới
- ☐ Tắt: Không hỏi

**Run on Windows Startup**
- ✓ Bật: Tự động chạy khi khởi động máy
- ☐ Tắt: Chạy thủ công

### Lưu cài đặt

- Click nút "Save"
- Cài đặt sẽ được lưu ngay

---

## 🔒 Tips Bảo mật

✅ **Nên làm**:
- Lưu mật khẩu mạnh (có chữ, số, ký tự đặc biệt)
- Chạy ứng dụng với quyền Admin
- Sao lưu file credentials.db định kỳ
- Sử dụng Windows password/PIN

❌ **Không nên làm**:
- Chia sẻ máy tính với người khác
- Bỏ máy tính mở khi không có ai trông
- Cài đặt phần mềm từ nguồn không tin cậy
- Bỏ qua cập nhật Windows

---

## 🗂️ Nơi lưu trữ dữ liệu

Ứng dụng lưu tất cả thông tin trên máy tính của bạn:

```
C:\Users\YourUsername\AppData\Roaming\Keypass\
├── credentials.db      (Các tài khoản, mật khẩu)
└── settings.json       (Cài đặt ứng dụng)
```

- **Không** upload lên cloud
- **Không** gửi cho ai khác
- **Nên** sao lưu định kỳ

---

## ❓ Câu hỏi thường gặp

**Q: Ứng dụng có an toàn không?**
> A: Có! Mật khẩu lưu trên máy tính của bạn, không gửi lên bất cứ server nào. Nhưng nên bảo vệ máy tính bằng password hoặc sinh trắc học.

**Q: Quên mật khẩu sao?**
> A: Keypass chỉ lưu, không thể khôi phục. Hãy giữ ghi chú an toàn hoặc sử dụng chức năng "Lấy lại mật khẩu" của dịch vụ đó.

**Q: Xóa file credentials.db sao?**
> A: Ứng dụng sẽ tạo mới. Dữ liệu cũ sẽ mất vĩnh viễn - hãy backup trước khi xóa!

**Q: Có thể dùng trên các máy khác không?**
> A: Được, nhưng phải copy file credentials.db. Không có tính năng đồng bộ mặc định.

**Q: Tại sao không phát hiện một số website?**
> A: Một số website tạo form theo cách đặc biệt. Hãy thêm thủ công hoặc chạy Admin.

---

## 📞 Hỗ trợ

Gặp vấn đề?

1. **Kiểm tra danh sách vấn đề phía trên**
   - Phần "Xử lý Sự cố" trong HUONG_DAN_VIETNAMESE.md

2. **Thử khôi phục cơ sở dữ liệu**
   - Xóa file credentials.db
   - Khởi động lại ứng dụng

3. **Chạy với quyền Admin**
   - Nhấp chuột phải vào .exe → Run as administrator

4. **Kiểm tra Windows Defender**
   - Có thể chặn ứng dụng
   - Thêm vào whitelist nếu cần

---

## 🎯 Tiếp theo?

Sau khi setup:
- ✅ Thêm 5-10 tài khoản thường dùng
- ✅ Bật "Run on Startup"
- ✅ Sao lưu file credentials.db
- ✅ Đọc full guide: HUONG_DAN_VIETNAMESE.md

---

**Cần thêm trợ giúp?** Xem các file:
- `README.md` - Tiếng Anh
- `HUONG_DAN_VIETNAMESE.md` - Tiếng Việt đầy đủ
- `BUILD_GUIDE.md` - Hướng dẫn build từ source

Chúc bạn sử dụng Keypass vui vẻ! 🎉

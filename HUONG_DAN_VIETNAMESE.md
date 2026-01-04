# Hướng dẫn sử dụng Keypass Password Manager

## Giới thiệu

**Keypass** là một ứng dụng quản lý mật khẩu chuyên nghiệp được viết bằng C# chạy trên nền tảng Windows. Ứng dụng chạy ẩn dưới thanh tray hệ thống và tự động phát hiện khi bạn đăng nhập vào các website hay ứng dụng, sau đó sẽ gợi ý lưu thông tin tài khoản/mật khẩu và hỗ trợ tự động điền thông tin đăng nhập.

## Tính năng chính

### 1. **Chạy trên System Tray**
- Ứng dụng chạy ẩn dưới thanh tray hệ thống
- Tiết kiệm tài nguyên và không làm gây nhiễu giao diện
- Dễ dàng truy cập bất cứ lúc nào qua icon trong tray

### 2. **Phát hiện Đăng nhập**
- Tự động phát hiện khi bạn truy cập vào các giao diện đăng nhập
- Giám sát liên tục các cửa sổ ứng dụng trên Windows
- Nhận biết các trang web hoặc ứng dụng yêu cầu tài khoản/mật khẩu

### 3. **Lưu Thông tin Đăng nhập**
- Khi bạn nhập tài khoản/mật khẩu lần đầu tiên:
  - Ứng dụng sẽ hiển thị popup hỏi có muốn lưu thông tin này không
  - Nếu chọn "Có" - thông tin sẽ được lưu vào cơ sở dữ liệu
  - Nếu chọn "Không" - sẽ không lưu

### 4. **Gợi ý Tự động (Auto-Suggest)**
- Lần thứ 2 khi truy cập vào giao diện đăng nhập của cùng một website/ứng dụng
- Khi bạn click vào ô nhập tài khoản (textbox), ứng dụng sẽ:
  - Tự động hiển thị popup đề xuất các tài khoản đã lưu
  - Chỉ hiển thị những tài khoản liên quan đến website/ứng dụng đó
  - Cho phép bạn chọn tài khoản muốn sử dụng

### 5. **Tự động Điền (Auto-Fill)**
- Sau khi chọn một tài khoản từ popup gợi ý:
  - Ứng dụng tự động điền tài khoản vào ô username
  - Tự động điền mật khẩu vào ô password
  - Bạn chỉ cần click nút "Đăng nhập" hoặc bấm Enter

### 6. **Quản lý Thông tin Đăng nhập**
- Xem danh sách tất cả thông tin đã lưu
- Thêm mới tài khoản/mật khẩu
- Chỉnh sửa thông tin hiện có
- Xóa thông tin không cần dùng nữa
- Tìm kiếm nhanh chóng theo tên website

### 7. **Cài đặt**
- Bật/tắt tính năng Auto-Fill
- Bật/tắt hỏi lưu thông tin mới
- Chọn chạy ứng dụng khi khởi động Windows

## Cách sử dụng

### Khởi động ứng dụng

1. Tìm và chạy file `Keypass.exe`
2. Ứng dụng sẽ khởi động và chạy trên thanh tray (dưới cùng bên phải màn hình)

### Quản lý Tài khoản/Mật khẩu

**Cách 1: Thêm tài khoản mới**
1. Truy cập vào website hoặc ứng dụng có yêu cầu đăng nhập
2. Nhập tài khoản và mật khẩu
3. Bấm Enter hoặc click nút đăng nhập
4. Popup xuất hiện hỏi "Bạn có muốn lưu mật khẩu này không?"
5. Chọn "Có" để lưu

**Cách 2: Thêm thủ công**
1. Nhấp đúp vào icon Keypass trong tray
2. Hoặc nhấp chuột phải vào icon > "Manage Passwords"
3. Click nút "Add"
4. Nhập Website, Username, Password
5. Click "Save"

### Sử dụng Auto-Fill

1. Truy cập vào giao diện đăng nhập
2. Click vào ô nhập tài khoản (Username field)
3. Popup sẽ hiện ra với danh sách tài khoản đã lưu
4. Click chọn tài khoản bạn muốn dùng
5. Ứng dụng tự động điền cả username và password
6. Click "Đăng nhập" hoặc bấm Enter

### Chỉnh sửa / Xóa Tài khoản

1. Mở Keypass Password Manager (nhấp đúp vào icon)
2. Tìm kiếm tài khoản bằng ô tìm kiếm (Search box)
3. Chọn tài khoản cần chỉnh sửa hoặc xóa
4. Click "Edit" để chỉnh sửa hoặc "Delete" để xóa

### Cài đặt Ứng dụng

1. Nhấp chuột phải vào icon Keypass trong tray
2. Chọn "Settings"
3. Điều chỉnh các tùy chọn:
   - **Enable Auto-Fill**: Bật/tắt tính năng gợi ý
   - **Ask to save new credentials**: Hỏi khi có tài khoản mới
   - **Run on Windows Startup**: Chạy tự động khi khởi động máy
4. Click "Save"

## Đặc điểm Kỹ thuật

### Công nghệ sử dụng
- **Ngôn ngữ**: C#
- **Framework**: .NET 6.0 Windows Forms
- **Cơ sở dữ liệu**: SQLite
- **OS**: Windows 7 trở lên

### Lưu trữ Dữ liệu
- Tất cả thông tin được lưu trữ cục bộ trên máy tính
- **Vị trí lưu**: `%APPDATA%\Keypass\credentials.db`
- **Cài đặt**: `%APPDATA%\Keypass\settings.json`

### Kiến trúc Ứng dụng
```
TrayApplicationContext
├── PasswordManagerForm (Quản lý mật khẩu)
├── AddEditPasswordForm (Thêm/Chỉnh sửa)
├── SettingsForm (Cài đặt)
└── Services
    ├── DatabaseService (Quản lý CSDL)
    ├── UIHookService (Phát hiện đăng nhập)
    └── SettingsService (Quản lý cài đặt)
```

## Bảo mật

### Khuyến nghị
1. **Bảo vệ máy tính**: Sử dụng mật khẩu Windows hoặc sinh trắc học
2. **Chạy với quyền Admin**: Để phát hiện chính xác hơn
3. **Antivirus**: Chạy phần mềm diệt virus thường xuyên
4. **Backup**: Sao lưu định kỳ file `credentials.db`

### Các tính năng bảo mật trong tương lai
- Mã hóa DPAPI cho thông tin đã lưu
- Mật khẩu chính (Master Password)
- Sinh trắc học (Windows Hello)
- Đồng bộ mã hóa giữa các thiết bị

## Xử lý Sự cố

### Ứng dụng không phát hiện trang đăng nhập
- **Nguyên nhân**: Một số trang web có cách tạo form đặc biệt
- **Giải pháp**: 
  - Chạy ứng dụng với quyền Admin
  - Kiểm tra Auto-Fill có được bật trong Settings
  - Thêm tài khoản thủ công

### Lỗi cơ sở dữ liệu
- **Nguyên nhân**: Quyền truy cập hoặc file bị hỏng
- **Giải pháp**:
  - Kiểm tra quyền ghi vào thư mục `%APPDATA%\Keypass\`
  - Kiểm tra Antivirus không chặn
  - Xóa file `credentials.db` và khởi động lại

### Mật khẩu không auto-fill
- **Nguyên nhân**: Tên website không khớp hoặc form custom
- **Giải pháp**:
  - Kiểm tra tên website lưu có chính xác
  - Thử tìm kiếm từ popup gợi ý thủ công
  - Nhập lại tài khoản nếu cần

## Yêu cầu Hệ thống

- **OS**: Windows 7, 8, 10, 11
- **.NET Runtime**: 6.0 trở lên
- **RAM**: 100 MB (tối thiểu)
- **Ổ cứng**: 50 MB (tối thiểu)
- **Quyền**: Quyền Admin (tùy chọn, tốt hơn với admin)

## Cài đặt & Chạy

### Từ Source Code

**Yêu cầu**:
- Visual Studio 2022 hoặc .NET 6.0 SDK

**Bước cài đặt**:
```bash
# Clone hoặc tải source code
cd Keypass

# Build
dotnet build

# Run
dotnet run

# Publish (Release)
dotnet publish -c Release -o ./publish
```

### Từ Executable

1. Tải file `Keypass.exe` 
2. Đặt ở vị trí muốn cài
3. Chạy file

## Hỗ trợ & Phản hồi

Nếu gặp vấn đề hoặc có đề xuất cải thiện, vui lòng:
- Kiểm tra mục "Xử lý Sự cố" phía trên
- Báo cáo lỗi chi tiết
- Gửi đề xuất tính năng mới

## Các phiên bản sắp tới

- [ ] Mã hóa nâng cao (DPAPI)
- [ ] Mật khẩu chính
- [ ] Tích hợp trình duyệt
- [ ] Sinh viết mật khẩu
- [ ] Kiểm tra độ mạnh mật khẩu
- [ ] Import/Export CSV
- [ ] Xác thực sinh trắc học
- [ ] Nhật ký đăng nhập

---

**Phiên bản**: 1.0.0  
**Cập nhật lần cuối**: 2024  
**Tác giả**: Keypass Team

> **Lưu ý**: Đây là ứng dụng cá nhân. Đối với sử dụng doanh nghiệp, hãy bổ sung các biện pháp bảo mật bổ sung.

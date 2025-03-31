# PosApp_Window_22KTPM1

### Hướng dẫn chạy project

#### **1. Clone và mở project**
- **Bước 1**: Clone project từ GitHub:  
  👉 [PosApp_Window_22KTPM1](https://github.com/thanhto24/PosApp_Window_22KTPM1.git)  
- **Bước 2**: Mở project bằng Visual Studio:  
  - Truy cập vào thư mục **App**  
  - Mở file **App.sln** bằng Visual Studio  
- **Bước 3**: Nhấn **Build** để tiến hành build project.  

---

#### **2. Chọn instance phù hợp**
- Truy cập vào **App > App.xaml.cs**, thay đổi giá trị **Singleton key**:

![alt text](image-2.png)

  - `'MockDao'`: Sử dụng dữ liệu giả, có thể chạy ngay sau bước 3.
  - `'MongoDao'`: Kết nối với database thật, cần cấu hình Backend.

📌 **Lưu ý**: Nếu chọn `'MongoDao'`, cần khởi động Backend Node.js trước khi chạy ứng dụng.

---

### **3. Cấu hình và chạy Backend**
##### **Bước 1:** Cài đặt Node.js trên máy.  
##### **Bước 2:** Mở thư mục **BE** bằng VS Code.  
##### **Bước 3:** Tạo file **.env** trong thư mục **BE** với nội dung sau:  
```sh
PORT=5678

MONGO_URI=mongodb+srv://thanhto:thanhto123@window.qb955.mongodb.net/newdb?retryWrites=true&w=majority&appName=Window
```
##### **Bước 4:** Mở terminal tại thư mục **BE** và chạy lệnh:  
```sh
npm i
```
(Để cài đặt các thư viện cần thiết.)  
##### **Bước 5:** Chạy Backend bằng lệnh:  
```sh
npm start
```
Hoặc  
```sh
node src/server.js
```
Nếu Backend khởi động thành công, terminal sẽ hiển thị:  
```
Server đang chạy tại http://localhost:5678  
Kết nối MongoDB thành công!
```
📌 **Lưu ý**: Giữ terminal mở trong suốt quá trình sử dụng ứng dụng để đảm bảo API luôn hoạt động.  

##### **Bước 6:** Để tắt Backend, nhấn tổ hợp phím:  
```
Ctrl + C (2 lần)
```

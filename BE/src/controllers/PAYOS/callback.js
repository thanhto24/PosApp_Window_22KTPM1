const { verifySignature } = require("./verifySignature");

const payosCallback = (req, res) => {
  if (!verifySignature(req)) {
    return res.status(403).json({ message: "Chữ ký không hợp lệ" });
  }

  const { orderCode, status } = req.body;

  if (status === "PAID") {
    console.log(`💵 Đơn hàng ${orderCode} đã thanh toán thành công!`);
    // 👉 Cập nhật trạng thái vào DB nếu có
  }

  res.status(200).json({ message: "Callback nhận thành công" });
};

module.exports = { payosCallback };

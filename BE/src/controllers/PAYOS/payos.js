const axios = require("axios");
require("dotenv").config();

const { generatePayloadWithSignature } = require("./createSignature");
const CLIENT_ID = process.env.PAYOS_CLIENT_ID;
const API_KEY = process.env.PAYOS_API_KEY;
const CHECKSUM_KEY = process.env.PAYOS_CHECK_SUM;
const PORT = process.env.PORT || 5678;
const CANCEL_URL =
  process.env.PAYOS_CANCEL_URL || `http://localhost:${PORT}/payos/cancel`;
const RETURN_URL =
  process.env.PAYOS_RETURN_URL || `http://localhost:${PORT}/payos/return`;

const headers = {
  "x-client-id": CLIENT_ID,
  "x-api-key": API_KEY,
  "Content-Type": "application/json",
};

// Tạo đơn hàng
const createPaymentLink = async (req, res) => {
  console.log("CHECK createPaymentLink: ", req.body);
  try {
    const orderCode = Date.now(); // Sử dụng thời gian hiện tại làm orderCode

    // const payload = {
    //   orderCode,
    //   amount: 100000, // Số tiền cần thanh toán
    //   description: "Thanh toán đơn hàng Node.js", // Mô tả đơn hàng
    //   returnUrl: "https://www.youtube.com/", // URL trả về khi thanh toán thành công
    //   cancelUrl: "https://www.facebook.com/", // URL khi khách hàng hủy thanh toán
    //   signature: "", // Chữ ký sẽ được tạo sau
    // };
    // console.log("CHECK orderCode: ", orderCode);
    // console.log("CHECK amount: ", req.body.amount || 10000);
    // console.log("Check Checksum key: ", CHECKSUM_KEY);

    const payloadWithSignature = generatePayloadWithSignature(
      orderCode,
      req.body.amount || 2222,
      "Thanh toán bằng mã QR",
      RETURN_URL,
      CANCEL_URL,
      CHECKSUM_KEY
    );
    console.log("CHECK payloadWithSignature: ", payloadWithSignature);
    const response = await axios.post(
      "https://api-merchant.payos.vn/v2/payment-requests",
      payloadWithSignature,
      { headers }
    );

    console.log("CHECK response: ", response.data);

    // Kiểm tra xem dữ liệu trả về có hợp lệ không
    if (!response.data || !response.data.data) {
      console.error("Dữ liệu trả về không hợp lệ: ", response.data);
      return res.status(500).json({
        error: "Không thể tạo đơn hàng, dữ liệu trả về không hợp lệ.",
      });
    }

    const { checkoutUrl, qrCode } = response.data.data;

    // Tự hủy sau 5 phút nếu chưa thanh toán
    setTimeout(async () => {
      console.log(
        `Hủy đơn hàng vì quá 5 phút chưa thanh toán: ${orderCode}`,
        "Link hủy đơn: " + `https://api-merchant.payos.vn/v2/payment-requests/${orderCode}/cancel`,
      );
      try {
        const statusRes = await axios.get(
          `https://api-merchant.payos.vn/v2/payment-requests/${orderCode}`, // Kiểm tra trạng thái thanh toán
          { headers }
        );

        const status = statusRes.data.data?.status;

        if (status !== "PAID") {
          // Hủy đơn nếu chưa thanh toán sau 5 phút
          await axios.post(
            `https://api-merchant.payos.vn/v2/payment-requests/${orderCode}/cancel`,
            {"cancellationReason":"Expired"},
            { headers }
        );
          console.log(`❌ Huỷ đơn hàng ${orderCode} sau 5 phút`);
        } else {
          console.log(`✅ Đơn hàng ${orderCode} đã thanh toán`);
        }
      } catch (e) {
        console.error("Lỗi khi kiểm tra hoặc huỷ đơn: ", e.message);
      }
    }, 5 * 60 * 1000); // Đợi 5 phút trước khi tự hủy

    // Trả về thông tin đơn hàng, URL thanh toán và QR code
    res.json({ orderCode, checkoutUrl, qrCode });
  } catch (err) {
    console.error("Lỗi khi tạo đơn hàng: ", err.response?.data || err.message);
    res.status(500).json({ error: "Không thể tạo đơn hàng" });
  }
};

// Kiểm tra trạng thái đơn hàng
const getOrderStatus = async (req, res) => {
  const { orderCode } = req.params;
  try {
    const response = await axios.get(
      `https://api-merchant.payos.vn/v2/payment-requests/${orderCode}`,
      { headers }
    );
    res.json(response.data.data.status);
  } catch (err) {
    res.status(500).json({ error: "Không kiểm tra được trạng thái" });
  }
};

module.exports = {
  createPaymentLink,
  getOrderStatus,
};

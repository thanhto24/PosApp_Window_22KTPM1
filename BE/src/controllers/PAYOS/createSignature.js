const crypto = require('crypto');

// Hàm tạo chữ ký HMAC-SHA256
const createSignature = (data, checksumKey) => {
  const hmac = crypto.createHmac('sha256', checksumKey);  // Tạo HMAC với SHA256
  hmac.update(data);  // Cập nhật dữ liệu cần mã hóa
  return hmac.digest('hex');  // Trả về chữ ký dưới dạng hex
};

// Hàm chuẩn bị dữ liệu và tạo chữ ký
const generatePayloadWithSignature = (orderCode, amount, description, returnUrl, cancelUrl, checksumKey) => {
  // Sắp xếp dữ liệu theo thứ tự alphabet
  const data = `amount=${amount}&cancelUrl=${cancelUrl}&description=${description}&orderCode=${orderCode}&returnUrl=${returnUrl}`;
  
  // Tạo chữ ký từ dữ liệu đã sắp xếp
  const signature = createSignature(data, checksumKey);

  // Trả về payload với chữ ký
  return {
    "orderCode": orderCode,
    "amount": amount,
    "description": description,
    "returnUrl": returnUrl,
    "cancelUrl": cancelUrl,
    "signature": signature,
  };
};

exports.generatePayloadWithSignature = generatePayloadWithSignature;
exports.createSignature = createSignature;
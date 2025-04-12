const crypto = require("crypto");
require("dotenv").config();
const CHECKSUM_KEY = process.env.PAYOS_CHECKSUM_KEY;

function verifySignature(req) {
  const signature = req.headers["x-signature"];
  const bodyString = JSON.stringify(req.body);
  const hmac = crypto.createHmac("sha256", CHECKSUM_KEY);
  const digest = hmac.update(bodyString).digest("hex");
  return digest === signature;
}

module.exports = { verifySignature };

const express = require("express");
const {
  createPaymentLink,
  getOrderStatus,
} = require("../controllers/PAYOS/payos");
const { payosCallback } = require("../controllers/PAYOS/callback");
const path = require("path");
const router = express.Router();

router.post("/create-payment", createPaymentLink);
router.post("/payos-callback", payosCallback);
router.get("/order-status/:orderCode", getOrderStatus);
router.get("/return", (req, res) => {
  res.sendFile(path.join(__dirname, "../views/return.html"));
});
router.get("/cancel", (req, res) => {
  res.sendFile(path.join(__dirname, "../views/cancel.html"));
});
module.exports = router;

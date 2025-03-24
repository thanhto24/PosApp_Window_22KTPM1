const express = require("express");
const router = express.Router();
const controller = require("../controllers/godController");

function validateModelName(req, res, next) {
    req.modelName = req.params.modelName.toLowerCase();
    if (!req.modelName) {
        return res.status(400).json({ error: "Model name is required!" });
    }
    next();
}

// Định nghĩa các route chuẩn RESTful API
router.post("/:modelName", validateModelName, controller.insert);         // Thêm mới
router.get("/:modelName", validateModelName, controller.getAll);          // Lấy toàn bộ
router.delete("/rmByQuery/:modelName", validateModelName, controller.removeByQuery);  // Xóa theo điều kiện
router.put("/updateByQuery/:modelName", validateModelName, controller.updateByQuery); // Cập nhật theo điều kiện

module.exports = router;

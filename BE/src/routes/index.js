const express = require("express");
const router = express.Router();
const controller = require("../controllers/godController");
const payosRouter = require("./payos");
const ghnRouter = require("./ghn");
function validateModelName(req, res, next) {
    req.modelName = req.params.modelName.toLowerCase();
    if (!req.modelName) {
        return res.status(400).json({ error: "Model name is required!" });
    }
    //Viet hoa chu dau trong req.body
    req.body = Object.keys(req.body).reduce((result, key) => {
        result[key.charAt(0).toUpperCase() + key.slice(1)] = req.body[key];
        return result;
    }, {});
    next();
}

// Định nghĩa các route chuẩn RESTful API
router.post("/:modelName", validateModelName, controller.insert);         // Thêm mới
router.get("/:modelName", validateModelName, controller.getAll);          // Lấy toàn bộ
router.delete("/rmByQuery/:modelName", validateModelName, controller.removeByQuery);  // Xóa theo điều kiện
router.put("/updateByQuery/:modelName", validateModelName, controller.updateByQuery); // Cập nhật theo điều kiện
router.post("/filtered/:modelName", validateModelName, controller.getAllFiltered);
router.post("/getByQuery/:modelName", validateModelName, controller.getByQuery);
router.use("/payos", payosRouter); // Sử dụng router payos
router.use("/ghn", ghnRouter); // Sử dụng router ghn
module.exports = router;

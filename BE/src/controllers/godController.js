const models = require("../models");

// 🟢 Hàm lấy model theo tên
const getModel = (modelName) => {
    const model = models[modelName];
    if (!model) throw new Error(`Model '${modelName}' không tồn tại`);
    return model;
};

// 🟢 Lấy danh sách (hỗ trợ phân trang, lọc, sắp xếp)
exports.getAll = async (req, res) => {
    try {
        const { modelName } = req.params;
        const Model = getModel(modelName);

        const data = await Model.find(); // Lấy toàn bộ dữ liệu
        res.json(data);
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
};


// 🟢 Thêm mới
exports.insert = async (req, res) => {
    try {
        const { modelName } = req.params;
        const Model = getModel(modelName);

        const entity = new Model(req.body);
        await entity.save();
        res.json({ message: "Thêm thành công!", entity });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
};

// 🟢 Xóa theo điều kiện
exports.removeByQuery = async (req, res) => {
    try {
        const { modelName } = req.params;
        const Model = getModel(modelName);

        const { whereClause } = req.body;
        if (!whereClause || Object.keys(whereClause).length === 0) {
            return res.status(400).json({ error: "Thiếu điều kiện xóa" });
        }

        const result = await Model.deleteMany(whereClause);
        res.json({ message: `Đã xóa ${result.deletedCount} bản ghi` });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
};

// 🟢 Cập nhật theo điều kiện
exports.updateByQuery = async (req, res) => {
    try {
        const { modelName } = req.params;
        const Model = getModel(modelName);

        const { setValues, whereClause } = req.body;
        if (!setValues || !whereClause) {
            return res.status(400).json({ error: "Thiếu dữ liệu cập nhật hoặc điều kiện" });
        }

        const result = await Model.updateMany(whereClause, { $set: setValues });
        res.json({ message: `Đã cập nhật ${result.modifiedCount} bản ghi` });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
};

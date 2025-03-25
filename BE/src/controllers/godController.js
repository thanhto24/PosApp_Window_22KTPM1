const models = require("../models");

// 🟢 Hàm lấy model theo tên
const getModel = (modelName) => {
  const model = models[modelName];
  if (!model) throw new Error(`Model '${modelName}' không tồn tại`);
  return model;
};

// 🟢 Lấy danh sách (hỗ trợ phân trang, lọc, sắp xếp)
exports.getAll = async (req, res) => {
  console.log("Get all");
  try {
    const { modelName } = req.params;
    console.log(modelName);
    const Model = getModel(modelName);

    const data = await Model.find(); // Lấy toàn bộ dữ liệu
    console.log(data);
    res.json(data);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
};

// 🟢 Thêm mới
exports.insert = async (req, res) => {
  console.log("Insert");
  try {
    const { modelName } = req.params;
    console.log(modelName);
    const Model = getModel(modelName);
    console.log("Body: ", req.body);
    const entity = new Model(req.body);
    console.log(entity);
    await entity.save();
    console.log("Save");
    res.json({ message: "Thêm thành công!", entity });
  } catch (error) {
    console.log(error);
    res.status(500).json({ error: error.message });
  }
};

// 🟢 Xóa theo điều kiện
exports.removeByQuery = async (req, res) => {
  console.log("Remove");
  try {
    const { modelName } = req.params;
    const Model = getModel(modelName);
    console.log("Body: ", req.body);
    const { WhereClause, Parameters } = req.body;

    if (!WhereClause) {
      return res.status(400).json({ error: "Thiếu điều kiện xóa" });
    }

    console.log("Before Replacement:", WhereClause);
    console.log("WhereParams:", Parameters);

    const mongoWhereClause = parseString(WhereClause, Parameters);
    console.log("MongoDB WhereClause:", mongoWhereClause);

    // Thực hiện xóa với MongoDB query
    const result = await Model.deleteMany(mongoWhereClause);
    console.log(result);

    res.json({ message: `Đã xóa ${result.deletedCount} bản ghi` });
  } catch (error) {
    console.log(error);
    res.status(500).json({ error: error.message });
  }
};

// 🟢 Cập nhật theo điều kiện
exports.updateByQuery = async (req, res) => {
  console.log("Update");
  try {
    const { modelName } = req.params;
    console.log(modelName);
    const Model = getModel(modelName);
    console.log("Body: ", req.body);

    const { SetValues, WhereClause, WhereParams } = req.body;
    console.log("Before Replacement:", WhereClause);
    console.log("WhereParams:", WhereParams);

    if (!SetValues || !WhereClause) {
      return res
        .status(400)
        .json({ error: "Thiếu dữ liệu cập nhật hoặc điều kiện" });
    }

    const mongoWhereClause = parseString(WhereClause, WhereParams);
    console.log("MongoDB WhereClause:", mongoWhereClause);

    // Gọi updateMany với MongoDB query
    const result = await Model.updateMany(mongoWhereClause, {
      $set: SetValues,
    });

    console.log(result);
    res.json({ message: `Đã cập nhật ${result.modifiedCount} bản ghi` });
  } catch (error) {
    console.log(error);
    res.status(500).json({ error: error.message });
  }
};

// 🟢 Hàm chuyển đổi điều kiện từ SQL sang MongoDB
const parseString = (WhereClause, WhereParams) => {
  // Thay thế tất cả @param bằng giá trị từ WhereParams
  let replacedClause = WhereClause.replace(/@\w+/g, (match) => {
    const paramName = match.substring(1);
    return WhereParams?.hasOwnProperty(paramName)
      ? WhereParams[paramName]
      : match;
  });

  console.log("Replaced WhereClause:", replacedClause);

  let conditions = {};
  let parts = replacedClause.split(/\s+AND\s+/);

  for (let part of parts) {
    let [key, value] = part.split("=").map((str) => str.trim());
    conditions[key] = isNaN(value) ? value : Number(value);
  }

  return {
    $and: Object.entries(conditions).map(([key, value]) => ({
      [key]: value,
    })),
  };
};

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

// 🟢 Lấy danh sách với hỗ trợ filter, sort
exports.getAllFiltered = async (req, res) => {
  try {
    const { modelName } = req.params;
    const { 
      SearchText, 
      ProductType, 
      ProductGroup, 
      Status, 
      SortOrder 
    } = req.body;

    console.log("CHECK req body filter: ", SearchText, ProductType, ProductGroup, Status, SortOrder);

    const Model = getModel(modelName);
    
    // Xây dựng điều kiện query
    let query = {};

    // Tìm kiếm theo text
    if (SearchText) {
      query.$or = [
        { Name: { $regex: SearchText, $options: 'i' } },
        { BarCode: { $regex: SearchText, $options: 'i' } },
        { Id: { $regex: SearchText, $options: 'i' } }
      ];
    }

    // Lọc theo loại sản phẩm
    if (ProductType && ProductType !== "Tất cả") {
      query.TypeGroup = ProductType;
    }

    // Lọc theo nhóm sản phẩm
    if (ProductGroup && ProductGroup !== "Tất cả") {
      query.TypeGroup = ProductGroup;
    }

    // Lọc theo trạng thái
    if (Status && Status !== "Tất cả") {
      query.InStock = Status === "Còn hàng" ? true : false;
    }

    console.log("CHECK QUERY: ", query);

    // Xác định sort
    let sort = { Name: 1 }; // Mặc định sắp xếp theo tên A-Z
    switch(SortOrder) {
      case "Tên: A => Z": sort = { Name: 1 }; break;
      case "Tên: Z => A": sort = { Name: -1 }; break;
      case "Giá: Thấp => Cao": sort = { Price: 1 }; break;
      case "Giá: Cao => Thấp": sort = { Price: -1 }; break;
      case "Ngày cập nhật: Cũ nhất": sort = { LastUpdated: 1 }; break;
      case "Ngày cập nhật: Mới nhất": sort = { LastUpdated: -1 }; break;
    }

    // Thực hiện query
    const data = await Model.find(query).sort(sort);
    
    res.json(data);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
};

exports.getByQuery = async (req, res) => {   
  try {     
      const { modelName } = req.params;     
      const query = req.body;     
      const Model = getModel(modelName);     
      console.log("Query: ", query);

      // Kiểm tra filter
      let mongoQuery = {};
      if (query.Filter && Object.keys(query.Filter).length > 0) {
          mongoQuery = convertToMongoQuery(query.Filter);
      }

      // Xử lý điều kiện OR nếu có
      if (query.Or && Object.keys(query.Or).length > 0) {
          const orConditions = Object.entries(query.Or).map(([key, value]) => {
              return convertToMongoQuery({ [key]: value });
          });

          mongoQuery["$or"] = orConditions;
      }

      // Thêm sắp xếp nếu có
      let sortOptions = {};
      if (query.Sort && Object.keys(query.Sort).length > 0) {
          sortOptions = query.Sort;
      }

      console.log("Mongo Query: ", JSON.stringify(mongoQuery, null, 2));

      // Thực thi truy vấn
      const result = await Model.find(mongoQuery).sort(sortOptions);
      console.log("Result: ", result);
      res.json(result);
  } catch (error) {     
      res.status(500).json({ error: error.message });   
  } 
};

function convertToMongoQuery(filter) {
  let mongoQuery = {};

  for (let key in filter) {
      let value = filter[key];

      // Chuyển `%` thành regex MongoDB
      if (typeof value === "string" && value.includes("%")) {
          let regexPattern = value.replace(/%/g, ".*"); // `%` thành `.*`
          mongoQuery[key] = { $regex: regexPattern, $options: "i" }; // Không phân biệt hoa thường
      } else {
          mongoQuery[key] = value;
      }
  }

  return mongoQuery;
}

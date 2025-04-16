require("dotenv").config();
const axios = require("axios");

const ghnAPI = axios.create({
  baseURL: process.env.GHN_BASE_URL,
  headers: {
    Token: process.env.GHN_API_TOKEN,
    ShopId: process.env.GHN_SHOP_ID,
  },
});

exports.calculateShippingFee = async (req, res) => {
  const {
    to_ward_code,
    to_district_id,
    weight = 1000,
    service_id = 53321,
    service_type_id = 3,
  } = req.body;

  try {
    const response = await ghnAPI.post("/v2/shipping-order/fee", {
      to_ward_code,
      to_district_id,
      weight,
      service_id: service_id,
      service_type_id: service_type_id,
    });
    const result = response.data.data;
    console.log(result);
    res.json({total_fee: result.total});
  } catch (err) {
    res.status(500).json({ error: err.response?.data || err.message });
  }
};

exports.createOrder = async (req, res) => {
  const {
    to_name,
    to_phone,
    to_address,
    to_ward_code,
    to_district_id,
    weight = 1000,
    length = 20,
    width = 15,
    height = 10,
    cod_amount = 0,
    content = "Coffee",
    client_order_code = "TEST_" + Date.now(),
    from_district_id = 1447,
    from_ward_code = "20504",
  } = req.body;

  try {
    const response = await ghnAPI.post("/v2/shipping-order/create", {
      to_name,
      to_phone,
      to_address,
      to_ward_code,
      to_district_id,
      cod_amount,
      content,
      weight,
      length,
      width,
      height,
      service_type_id: 2,
      payment_type_id: 1,
      required_note: "CHOXEMHANGKHONGTHU",
      client_order_code,
      from_district_id,
      from_ward_code,
      insurance_value: cod_amount,
    });
    const result = response.data.data;
    console.log(result);
    res.json({
      order_code: result.order_code,
      total_fee: result.total_fee,
    });
  } catch (err) {
    res.status(500).json({ error: err.response?.data || err.message });
  }
};

exports.getProvinces = async (req, res) => {
  try {
    const response = await ghnAPI.get("/master-data/province");

    const provinces = response.data.data
      .filter((p) => !p.ProvinceName.toLowerCase().includes("test"))
      .map((p) => ({
        id: p.ProvinceID,
        name: p.ProvinceName,
      }))
      .sort((a, b) => a.name.localeCompare(b.name)); // <-- Sắp xếp theo tên

    console.log(provinces);
    res.json(provinces);
  } catch (err) {
    res.status(500).json({ error: err.response?.data || err.message });
  }
};

exports.getDistrictsByProvince = async (req, res) => {
  const { province_id } = req.query;

  if (!province_id) {
    return res.status(400).json({ error: "Missing province_id" });
  }

  try {
    const response = await ghnAPI.get("/master-data/district", {
      params: { province_id },
    });
    const districts = response.data.data
      .map((d) => ({
        id: d.DistrictID,
        name: d.DistrictName,
        province_id: d.ProvinceID,
      }))
      .sort((a, b) => a.name.localeCompare(b.name));
    console.log(districts);
    res.json(districts);
  } catch (err) {
    res.status(500).json({ error: err.response?.data || err.message });
  }
};

exports.getWardsByDistrict = async (req, res) => {
  const { district_id } = req.query;

  if (!district_id) {
    return res.status(400).json({ error: "Missing district_id" });
  }

  try {
    const response = await ghnAPI.get("/master-data/ward", {
      params: { district_id },
    });
    const wards = response.data.data
      .map((w) => ({
        id: w.WardCode,
        name: w.WardName,
        district_id: w.DistrictID,
      }))
      .sort((a, b) => a.name.localeCompare(b.name));

    console.log(wards);
    res.json(wards);
  } catch (err) {
    res.status(500).json({ error: err.response?.data || err.message });
  }
};

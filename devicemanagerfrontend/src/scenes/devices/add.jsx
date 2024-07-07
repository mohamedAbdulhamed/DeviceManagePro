import { Box, Button, TextField, MenuItem, Typography } from "@mui/material";
import { Formik } from "formik";
import * as yup from "yup";
import useMediaQuery from "@mui/material/useMediaQuery";
import Header from "../../components/Header";
import axios, { AxiosError } from "axios";
import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { getToken } from "../../utils/auth"
import Alert from '@mui/material/Alert';

const RegisterForm = () => {
  const isNonMobile = useMediaQuery("(min-width:600px)");
  const navigate = useNavigate();
  const [deviceTypes, setDeviceTypes] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(null);
  const [initialValues, setInitialValues] = useState({
    serialno: "",
    name: "",
    typeId: "",
  });

  useEffect(() => {
    const fetchDeviceTypes = async () => {
      setLoading(true);

      try {
        const token = getToken();
        if (token === null) navigate("/login");

        const response = await axios.get("https://localhost:7001/api/Device/GetAllDeviceTypes", {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });

        const types = response.data.map((type) => ({
          value: type.id,
          label: type.name,
        }));

        setDeviceTypes(types);

        if (types.length > 0) {
          setInitialValues((prevValues) => ({
            ...prevValues,
            type: types[0].value,
          }));
        }

      } catch (err) {
        if (err && err instanceof AxiosError) setError(err.response?.data);
        else if (err && err instanceof Error) setError(err.message);
        console.error("Error fetching device types: ", err);
      }

      setLoading(false);
    };

    fetchDeviceTypes();
  }, [navigate]);
  
  const handleFormSubmit = async (values) => {
    setLoading(true);
    setError(null);
    setSuccess(null);

    try {
      const token = getToken();
      if (token === null) navigate("/login");

      const response = await axios.post("https://localhost:7001/api/Device/Add", values, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (response.status === 201)
      {
        setSuccess("Device added successfully! redirecting you to /devices");
        setLoading(false);
        setTimeout(() => {
          navigate("/devices");
        }, 2000);
      }else {
        setLoading(false);
        setError("Couldn't add device, Try again later!");
        console.error("result status code is not 201");
      }
    }  catch (err) {
      setLoading(false);
      if (err && err instanceof AxiosError) setError(err.response?.data);
      else if (err && err instanceof Error) setError(err.message);
      console.error("Error: ", err);
    }
  };

  if (loading) return <Typography>Loading...</Typography>;

  return (
    <Box m="20px">
      <Header title="CREATE Device" subtitle="Add New Device" />

      {error && Array.isArray(error) && error.map((errMsg, index) => (
        <Alert key={index} severity="error" sx={{ marginBottom: '50px' }}>
          {errMsg}
        </Alert>
      ))}
      {success && <Alert severity="success" sx={{ marginBottom: '50px' }}>{success}</Alert>}

      <Formik
        onSubmit={handleFormSubmit}
        initialValues={initialValues}
        validationSchema={checkoutSchema}
      >
        {({
          values,
          errors,
          touched,
          handleBlur,
          handleChange,
          handleSubmit,
        }) => (
          <form onSubmit={handleSubmit}>
            <Box
              display="grid"
              gap="30px"
              gridTemplateColumns="repeat(4, minmax(0, 1fr))"
              sx={{
                "& > div": { gridColumn: isNonMobile ? undefined : "span 4" },
              }}
            >
              <TextField
                fullWidth
                variant="filled"
                type="text"
                label="Serial No."
                onBlur={handleBlur}
                onChange={handleChange}
                value={values.serialno}
                name="serialno"
                error={!!touched.serialno && !!errors.serialno}
                helperText={touched.serialno && errors.serialno}
                sx={{ gridColumn: "span 2" }}
              />
              <TextField
                fullWidth
                variant="filled"
                type="text"
                label="Name"
                onBlur={handleBlur}
                onChange={handleChange}
                value={values.name}
                name="name"
                error={!!touched.name && !!errors.name}
                helperText={touched.name && errors.name}
                sx={{ gridColumn: "span 2" }}
              />
              <TextField
                select
                fullWidth
                variant="filled"
                label="Device Type"
                onBlur={handleBlur}
                onChange={handleChange}
                value={values.typeId}
                name="typeId"
                error={!!touched.typeId && !!errors.typeId}
                helperText={touched.typeId && errors.typeId}
                sx={{ gridColumn: "span 4" }}
              >
                {deviceTypes.map((option) => (
                  <MenuItem key={option.value} value={option.value}>
                    {option.label}
                  </MenuItem>
                ))}
              </TextField>
            </Box>
            <Box display="flex" justifyContent="end" mt="20px">
              <Button type="submit" color="secondary" variant="contained">
                Create New Device
              </Button>
            </Box>
          </form>
        )}
      </Formik>
    </Box>
  );
};

const checkoutSchema = yup.object().shape({
  serialno: yup.string().required("required"),
  name: yup.string().required("required"),
  typeId: yup.string().required("required"),
});

export default RegisterForm;

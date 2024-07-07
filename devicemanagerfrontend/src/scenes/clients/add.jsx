import { Box, Button, TextField, Typography } from "@mui/material";
import { Formik } from "formik";
import { useState } from "react";
import * as yup from "yup";
import useMediaQuery from "@mui/material/useMediaQuery";
import Header from "../../components/Header";
import axios, { AxiosError } from "axios";
import { useNavigate } from "react-router-dom";
import { getToken } from "../../utils/auth"
import Alert from '@mui/material/Alert';


const AddClient = () => {
  const navigate = useNavigate();
  const isNonMobile = useMediaQuery("(min-width:600px)");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(null);

  const handleFormSubmit = async (values) => {
    setLoading(true);
    setError(null);
    setSuccess(null);

    const stringValues = {
      ...values,
      nationalId: values.nationalId.toString(),
      latitude: values.latitude.toString(),
      longitude: values.longitude.toString(),
    };

    try {
      const token = getToken();
      if (token === null) navigate("/login");

      const response = await axios.post("https://localhost:7001/api/Client/Add", stringValues, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (response.status === 201)
      {
        setSuccess("Client added successfully! redirecting you to /clients");
        setLoading(false);
        setTimeout(() => {
          navigate("/clients");
        }, 2000);
      }else {
        setLoading(false);
        setError("Couldn't add client, Try again later!");
        console.error("result status code is not 201");
      }
    }  catch (err) {
      setLoading(false);
      if (err && err instanceof AxiosError) {
        const errors = err.response?.data.errors;
        if (errors) {
          setError(Object.values(errors).flat());
        } else {
          setError("An error occurred");
        }
      } else if (err && err instanceof Error) {
        setError(err.message);
      }
      console.error("Error: ", err);
    }
  };

  if (loading) return <Typography>Loading...</Typography>;

  return (
    <Box m="20px">
      <Header title="Add Client" subtitle="Create a New Client" />

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
                fullWidth
                variant="filled"
                type="number"
                label="National ID"
                onBlur={handleBlur}
                onChange={handleChange}
                value={values.email}
                name="nationalId"
                error={!!touched.nationalId && !!errors.nationalId}
                helperText={touched.nationalId && errors.nationalId}
                sx={{ gridColumn: "span 2" }}
              />
              <TextField
                fullWidth
                variant="filled"
                type="number"
                label="Latitude"
                onBlur={handleBlur}
                onChange={handleChange}
                value={values.latitude}
                name="latitude"
                error={!!touched.latitude && !!errors.latitude}
                helperText={touched.latitude && errors.latitude}
                sx={{ gridColumn: "span 1" }}
              />
              <TextField
                fullWidth
                variant="filled"
                type="number"
                label="Longitude"
                onBlur={handleBlur}
                onChange={handleChange}
                value={values.longitude}
                name="longitude"
                error={!!touched.longitude && !!errors.longitude}
                helperText={touched.longitude && errors.longitude}
                sx={{ gridColumn: "span 1" }}
              />
            </Box>
            <Box display="flex" justifyContent="end" mt="20px">
              <Button type="submit" color="secondary" variant="contained">
                Create New Client
              </Button>
            </Box>
          </form>
        )}
      </Formik>
    </Box>
  );
};

const checkoutSchema = yup.object().shape({
  name: yup.string().required("required"),
  nationalId: yup
  .string()
  .required("National ID is required")
  .min(14, "National ID must be at least 14 characters")
  .max(20, "National ID must not exceed 20 characters"),
  latitude: yup
    .number()
    .min(-90, "Latitude must be between -90 and 90")
    .max(90, "Latitude must be between -90 and 90")
    .required("required"),
  longitude: yup
    .number()
    .min(-180, "Longitude must be between -180 and 180")
    .max(180, "Longitude must be between -180 and 180")
    .required("required"),
});

const initialValues = {
  name: "",
  nationalId: "",
  latitude: 0.0,
  longitude: 0.0,
};

export default AddClient;

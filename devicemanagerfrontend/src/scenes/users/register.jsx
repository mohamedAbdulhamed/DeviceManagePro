import { Box, Button, TextField, Typography } from "@mui/material";
import { Formik } from "formik";
import * as yup from "yup";
import useMediaQuery from "@mui/material/useMediaQuery";
import Header from "../../components/Header";
import axios, { AxiosError } from "axios";
import { useNavigate } from "react-router-dom";
import { useState } from "react";
import { getToken } from "../../utils/auth"
import Alert from '@mui/material/Alert';


const RegisterForm = () => {
  const isNonMobile = useMediaQuery("(min-width:600px)");
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(null);

  
  const handleFormSubmit = async (values) => {
    setLoading(true);
    setError(null);
    setSuccess(null);

    try {
      const token = getToken();
      if (token === null) navigate("/login");

      const response = await axios.post("https://localhost:7001/api/Account/register", values, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (response.status === 200)
      {
        setSuccess("User added successfully! redirecting you to /users");
        setLoading(false);
        setTimeout(() => {
          navigate("/users");
        }, 2000);
      }else {
        setLoading(false);
        setError("Couldn't add user, Try again later!");
        console.error("Login status code is not 201");
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
      <Header title="CREATE USER" subtitle="Register New User" />

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
                label="Username"
                onBlur={handleBlur}
                onChange={handleChange}
                value={values.username}
                name="username"
                error={!!touched.username && !!errors.username}
                helperText={touched.username && errors.username}
                sx={{ gridColumn: "span 2" }}
              />
              <TextField
                fullWidth
                variant="filled"
                type="password"
                label="Password"
                onBlur={handleBlur}
                onChange={handleChange}
                value={values.password}
                name="password"
                error={!!touched.password && !!errors.password}
                helperText={touched.password && errors.password}
                sx={{ gridColumn: "span 2" }}
              />
              <TextField
                fullWidth
                variant="filled"
                type="text"
                label="First Name"
                onBlur={handleBlur}
                onChange={handleChange}
                value={values.firstName}
                name="firstName"
                error={!!touched.firstName && !!errors.firstName}
                helperText={touched.firstName && errors.firstName}
                sx={{ gridColumn: "span 2" }}
              />
              <TextField
                fullWidth
                variant="filled"
                type="text"
                label="Last Name"
                onBlur={handleBlur}
                onChange={handleChange}
                value={values.lastName}
                name="lastName"
                error={!!touched.lastName && !!errors.lastName}
                helperText={touched.lastName && errors.lastName}
                sx={{ gridColumn: "span 2" }}
              />
              <TextField
                fullWidth
                variant="filled"
                type="text"
                label="Email"
                onBlur={handleBlur}
                onChange={handleChange}
                value={values.email}
                name="email"
                error={!!touched.email && !!errors.email}
                helperText={touched.email && errors.email}
                sx={{ gridColumn: "span 2" }}
              />

            </Box>
            <Box display="flex" justifyContent="end" mt="20px">
              <Button type="submit" color="secondary" variant="contained">
                Create New User
              </Button>
            </Box>
          </form>
        )}
      </Formik>
    </Box>
  );
};

const checkoutSchema = yup.object().shape({
  username: yup.string().required("required"),
  firstName: yup.string().required("required"),
  lastName: yup.string().required("required"),
  email: yup.string().email("invalid email").required("required"),
  password: yup.string().min(8, "Password must be at least 8 characters").required("required"),
});

const initialValues = {
  username: "",
  firstName: "",
  lastName: "",
  email: "",
  password: "",
};

export default RegisterForm;

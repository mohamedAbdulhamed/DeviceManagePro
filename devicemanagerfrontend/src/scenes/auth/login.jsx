import { Box, Button, TextField, Typography, useTheme } from "@mui/material";
import { Formik } from "formik";
import * as yup from "yup";
import useMediaQuery from "@mui/material/useMediaQuery";
import Header from "../../components/Header";
import { useState, useEffect } from "react";
import axios, { AxiosError } from "axios";
import { useNavigate } from "react-router-dom";
import { setToken, setRefreshToken, isAuthenticated } from "../../utils/auth";
import { useAuth } from "../../context/AuthContext";
import { tokens } from "../../theme";

const Login = () => {
  const theme = useTheme();
  const colors = tokens(theme.palette.mode);
  const isNonMobile = useMediaQuery("(min-width:600px)");
  const navigate = useNavigate();
  const { login } = useAuth();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (isAuthenticated()) navigate('/dashboard');
  });

  const handleFormSubmit = async (values) => {
    setLoading(true);
    setError("");
    console.log("Values: ", values);

    try {
      const response = await axios.post("https://localhost:7001/api/Account/login", values);
      if (response.status === 200)
      {
        setToken(response.data.token);
        setRefreshToken(response.data.token);
        login();
        navigate("/dashboard");
      }else {
        setLoading(false);
        setError("Login failed");
        console.error("Login status code is not 200");
      }
    } catch (err) {
      setLoading(false);
      if (err && err instanceof AxiosError) setError(err.response?.data);
      else if (err && err instanceof Error) setError(err.message);
      console.error("Error: ", err);
    }

  };

  if (loading) return <Typography>Loading...</Typography>;
  

  return (
    <Box m="20px">
      <Header title="Login" subtitle="Login to access your dashboard." />

      {error && <Typography color={colors.redAccent[500]} variant="h3" >{error}</Typography>}

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
            </Box>
            <Box display="flex" justifyContent="end" mt="20px">
              <Button type="submit" color="secondary" variant="contained">
                Login
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
  password: yup.string().required("required"),
});

const initialValues = {
  username: "",
  password: "",
};

export default Login;

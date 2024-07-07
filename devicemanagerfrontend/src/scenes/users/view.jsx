import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import axios from "axios";
import { Typography, CircularProgress, Paper, Container } from "@mui/material";
import { useTheme } from "@mui/material/styles";
import { tokens } from "../../theme";
import { getToken } from "../../utils/auth";

const UserView = () => {
  const { id } = useParams();
  const theme = useTheme();
  const colors = tokens(theme.palette.mode);

  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchUser = async () => {
      try {
        const token = getToken();
        if (!token) {
          console.error("token is null");
          return;
        }
        const response = await axios.get(
          `https://localhost:7001/api/Account/GetUserById/${id}`,
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );
        setUser(response.data);
        setLoading(false);
      } catch (err) {
        setError(err);
        setLoading(false);
      }
    };

    fetchUser();
  }, [id]);

  if (loading)
    return (
      <Typography color={colors.blueAccent[500]}>
        Loading... <CircularProgress />
      </Typography>
    );
  if (error)
    return (
      <Typography color={colors.redAccent[500]}>
        Error: {error.message}
      </Typography>
    );

  return (
    <Container sx={{ mt: 4 }}>
      <Paper sx={{ p: 3 }}>
        <Typography
          variant="h4"
          gutterBottom
          sx={{ color: colors.primary[500] }}
        >
          User Details
        </Typography>
        <Typography variant="h6" gutterBottom>
          Name: {user.firstName + " " + user.lastName } 
        </Typography>
        <Typography variant="body1" gutterBottom>
          User Name: {user.userName}
        </Typography>
        <Typography variant="body1" gutterBottom>
          Email: {user.email}
        </Typography>
      </Paper>
    </Container>
  );
};

export default UserView;

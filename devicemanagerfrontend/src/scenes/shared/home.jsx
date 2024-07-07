import React from 'react';
import { AppBar, Toolbar, Typography, Button, Box, useTheme } from '@mui/material';
import { tokens } from "../../theme";
import { isAuthenticated } from "../../utils/auth"
import Copyright from '../../components/Copyright';

const Home = () => {
  const theme = useTheme();
  const colors = tokens(theme.palette.mode);

  return (
    <Box
      sx={{
        display: 'flex',
        flexDirection: 'column',
        minHeight: '90vh',
      }}
    >
      <AppBar position="static" sx={{ bgcolor: colors.primary[500] }}>
        <Toolbar>
          <Typography variant="h6" sx={{ flexGrow: 1, color: colors.grey[100] }}>
            Home
          </Typography>
          <Button color="inherit" href={isAuthenticated() ? "/dashboard" : "/login"} sx={{ color: colors.grey[100] }}>
            {isAuthenticated() ? "To The Dashboard" : "Login"}
          </Button>
        </Toolbar>
      </AppBar>

      <Box
        sx={{
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          justifyContent: 'center',
          flexGrow: 1,
          backgroundImage: 'url(https://source.unsplash.com/random)',
          backgroundSize: 'cover',
          color: colors.grey[100],
        }}
      >
        <Typography variant="h2" component="h1" gutterBottom>
          Welcome to Device Manager Pro
        </Typography>
        <Typography variant="h5" component="h2" gutterBottom>
          Control the devices and their users.
        </Typography>
        <Button variant="contained" sx={{ bgcolor: colors.redAccent[500], color: colors.grey[100] }} href={isAuthenticated() ? "/dashboard" : "/login"}>
          {isAuthenticated() ? "Head to your dashboard" : "Get Started"}
        </Button>
      </Box>

      <Box sx={{ bgcolor: colors.grey[900], p: 6 }} component="footer">
        <Typography variant="h6" align="center" gutterBottom sx={{ color: colors.grey[100] }}>
          Device Manager
        </Typography>
        <Typography variant="subtitle1" align="center" color={colors.grey[500]} component="p">
          Have control over everything and more!
        </Typography>
        <Copyright sx={{ color: colors.grey[100] }} />
      </Box>
    </Box>
  );
};

export default Home;

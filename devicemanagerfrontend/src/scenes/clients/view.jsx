import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { Typography, CircularProgress, Paper, Container, Box } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import { tokens } from '../../theme';
import { getToken } from "../../utils/auth";
import { useParams, Link } from 'react-router-dom';

const ClientView = () => {
  const { id } = useParams();
  const theme = useTheme();
  const colors = tokens(theme.palette.mode);

  const [client, setClient] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchClient = async () => {
      try {
        const token = getToken();
        if (!token) {
          console.error("token is null");
          return;
        }
        const response = await axios.get(`https://localhost:7001/api/Client/GetById/${id}`, {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        setClient(response.data);
        setLoading(false);
      } catch (err) {
        setError(err);
        setLoading(false);
      }
    };

    fetchClient();
  }, [id]);

  if (loading) {
    return (
      <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', height: '100vh' }}>
        <CircularProgress color="primary" />
        <Typography color={colors.blueAccent[500]} sx={{ ml: 2 }}>Loading...</Typography>
      </Box>
    );
  }

  if (error) {
    return (
      <Typography color={colors.redAccent[500]}>
        Error: {error.message}
      </Typography>
    );
  }

  return (
    <Container sx={{ mt: 4 }}>
      <Paper sx={{ p: 3 }}>
        <Typography variant="h4" gutterBottom sx={{ color: colors.primary[500] }}>
          Client Details
        </Typography>
        <Typography variant="h6" gutterBottom>
          Name: {client.name}
        </Typography>
        <Typography variant="body1" gutterBottom>
          National ID: {client.nationalId}
        </Typography>
        <Typography variant="body1" gutterBottom>
          Latitude: {client.latitude}
        </Typography>
        <Typography variant="body1" gutterBottom>
          Longitude: {client.longitude}
        </Typography>
        <Typography variant="body1" gutterBottom>
          Created By: <Link to={`/user/${client.createdBy}`} style={{ color: colors.blueAccent[500] }}>{client.createdBy}</Link>
        </Typography>
        <Typography variant="body1" gutterBottom>
          Created At: {new Date(client.createdAt).toLocaleDateString()}
        </Typography>
        <Typography variant="body1" gutterBottom>
          Updated At: {new Date(client.updatedAt).toLocaleDateString()}
        </Typography>

        {client.devices && client.devices.length > 0 && (
          <>
            <Typography variant="h5" gutterBottom sx={{ mt: 4 }}>
              Devices
            </Typography>
            {client.devices.map((device) => (
              <Paper key={device.id} sx={{ p: 2, mb: 2 }}>
                <Typography variant="h6" gutterBottom>
                  Device Name: {device.name}
                </Typography>
                <Typography variant="body1" gutterBottom>
                  Serial Number: {device.serialNo}
                </Typography>
                <Typography variant="body1" gutterBottom>
                  Status: {device.status}
                </Typography>
                <Typography variant="body1" gutterBottom>
                  Created At: {new Date(device.createdAt).toLocaleDateString()}
                </Typography>
                <Typography variant="body1" gutterBottom>
                  Updated At: {new Date(device.updatedAt).toLocaleDateString()}
                </Typography>
              </Paper>
            ))}
          </>
        )}
      </Paper>
    </Container>
  );
};

export default ClientView;

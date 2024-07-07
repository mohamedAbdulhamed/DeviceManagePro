import { Box, Typography, useTheme, Button } from "@mui/material";
import { DataGrid, GridToolbar } from "@mui/x-data-grid";
import { tokens } from "../../theme";
import Header from "../../components/Header";
import React, { useState, useEffect } from "react";
import { getToken } from "../../utils/auth"
import axios, { AxiosError } from "axios";
import { useNavigate } from "react-router-dom";

const Devices = () => {
  const theme = useTheme();
  const colors = tokens(theme.palette.mode);
  const [devices, setDevices] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  const fetchDevices = async () => {
    setLoading(true);
    try {
      const token = getToken();
      if (token === null) console.error("token is null");
      const response = await axios.get("https://localhost:7001/api/Device/GetAll", {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
      const data = response.data.map(device => ({
        ...device,
        status: device.status === 1 ? "On" : "Off",
        createdAt: new Date(device.createdAt + "T12:00:00"),
        updatedAt: new Date(device.updatedAt + "T12:00:00"),
        type: device.type.name,
      }));
      setDevices(data);
      setLoading(false);
    } catch (err) {
      if (err && err instanceof AxiosError) setError(err.response?.data);
      else if (err && err instanceof Error) setError(err.message);
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchDevices();
  }, []);

  const columns = [
    { field: "id", headerName: "ID" },
    {
      field: "serialNo",
      headerName: "Serial No.",
      flex: 1,
      cellClassName: "name-column--cell",
    },
    {
      field: "name",
      headerName: "Name",
      flex: 1,
      cellClassName: "name-column--cell",
    },
    {
      field: "status",
      headerName: "Status",
      flex: 1,
      cellClassName: (params) =>
        params.value === "On" ? "status-on" : "status-off",
    },
    {
      field: "createdAt",
      headerName: "Created At",
      type: "date",
      flex: 1,
      cellClassName: "name-column--cell",
    },
    {
      field: "updatedAt",
      headerName: "Updated At",
      type: "date",
      flex: 1,
      cellClassName: "name-column--cell",
    },
    {
      field: "createdBy",
      headerName: "CreatedBy",
      flex: 1,
      cellClassName: "name-column--cell",
    },
    {
      field: "type",
      headerName: "Type",
      flex: 1,
      cellClassName: "name-column--cell",
    },
    {
      field: "actions",
      headerName: "Actions",
      flex: 1,
      renderCell: (params) => (
        <Button
          variant="contained"
          sx={{
            backgroundColor: colors.redAccent[500],
            color: colors.grey[100],
            borderRadius: "4px",
          }}
          onClick={() => handleDelete(params.row.id)}
        >
          Delete
        </Button>
      ),
    },
  ];

  const handleDelete = async (id) => {
    try {
      const token = getToken();
      if (token === null) navigate("/login");

      await axios.delete(`https://localhost:7001/api/Device/Delete/${id}`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
      fetchDevices();
    } catch (err) {
      if (err && err instanceof AxiosError) setError(err.response?.data.error);
      else if (err && err instanceof Error) setError(err.message);
    }
  };

  if (loading) return <Typography>Loading...</Typography>;


  return (
    <Box m="20px">
      <Header title="Devices" subtitle="Managing devices" />

      {error && <Typography color={colors.redAccent[500]} variant="h3" >{error}</Typography>}

      <Box
        m="40px 0 0 0"
        height="75vh"
        sx={{
          "& .MuiDataGrid-root": {
            border: "none",
          },
          "& .MuiDataGrid-cell": {
            borderBottom: "none",
          },
          "& .name-column--cell": {
            color: colors.greenAccent[300],
          },
          "& .status-on": {
            color: colors.greenAccent[500],
          },
          "& .status-off": {
            color: colors.redAccent[500],
          },
          "& .MuiDataGrid-columnHeaders": {
            backgroundColor: colors.blueAccent[700],
            borderBottom: "none",
          },
          "& .MuiDataGrid-virtualScroller": {
            backgroundColor: colors.primary[400],
          },
          "& .MuiDataGrid-footerContainer": {
            borderTop: "none",
            backgroundColor: colors.blueAccent[700],
          },
          "& .MuiCheckbox-root": {
            color: `${colors.greenAccent[200]} !important`,
          },
          "& .MuiDataGrid-toolbarContainer .MuiButton-text": {
            color: `${colors.grey[100]} !important`,
          },
        }}
      >
        <DataGrid rows={devices} columns={columns} components={{ Toolbar: GridToolbar }} />
      </Box>
    </Box>
  );
};

export default Devices;

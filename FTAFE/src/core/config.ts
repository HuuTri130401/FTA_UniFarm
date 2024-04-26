export const config = {
    SERVER_URL: process.env.SERVER_URL,
    // SERVER_URL: process.env.SERVER_URL || 'https://tripper.monoinfinity.net/api/v1.0',
    SERVER_SOCKET_URL: process.env.SERVER_SOCKET_URL || 'https://trip.monoinfinity.net',
    SERVER_SOCKET_PATH: process.env.SERVER_SOCKET_PATH || '/socket.io',
    minDistance: process.env.MIN_DISTANCE || 20000,
};

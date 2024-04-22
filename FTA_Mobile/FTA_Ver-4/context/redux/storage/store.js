// import {
//   FLUSH,
//   PAUSE,
//   PERSIST,
//   PURGE,
//   REGISTER,
//   REHYDRATE,
//   persistReducer,
//   persistStore,
// } from "redux-persist";
// import { configureStore } from "@reduxjs/toolkit";
//
// import rootReducer from "../reducers";
// import AsyncStorage from "@react-native-async-storage/async-storage";
//
// // Configure Redux Persist
// const persistConfig = {
//   key: "root", // key to save the Redux state to AsyncStorage
//   storage: AsyncStorage, // use AsyncStorage as the storage engine
//   whitelist: ["cart"], // specify which parts of the Redux state to persist
//   // blacklist: [''], // alternatively, specify which parts of the Redux state not to persist
// };
//
// // Create a persisted reducer
// const persistedReducer = persistReducer(persistConfig, rootReducer);
//
// // Create the Redux store
// const store = configureStore({
//   reducer: persistedReducer,
//   middleware: (getDefaultMiddleware) =>
//     getDefaultMiddleware({
//       serializableCheck: {
//         ignoredActions: [FLUSH, REHYDRATE, PAUSE, PERSIST, PURGE, REGISTER],
//       },
//     }),
// });
//
// // Create a persisted store
// const persistor = persistStore(store);
//
// export { store, persistor };

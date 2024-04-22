// const initialState = {
//   farmhubs: [],
// };
//
// const cartReducer = (state = initialState, action) => {
//   switch (action.type) {
//     case "ADD_TO_CART":
//       if (Array.isArray(state.farmhubs) && state.farmhubs.length > 0) {
//         return {
//           ...state,
//           farmhubs: state.farmhubs.map((farmhub) =>
//             farmhub.id === action.payload.id
//               ? {
//                   ...farmhub,
//                   prodItems: [...farmhub.prodItems, action.payload.prodItems],
//                 }
//               : farmhub,
//           ),
//         };
//       } else {
//         return {
//           ...state,
//           farmhubs: [...state.farmhubs, action.payload],
//         };
//       }
//     case "UPDATE_CART":
//       return {
//         ...state,
//         farmhubs: state.farmhubs.map(
//           (farmhub) =>
//             farmhub.id === action.payload.farmhubId && {
//               ...farmhub,
//               prodItems: farmhub.prodItems.map(
//                 (item) =>
//                   item.id === action.payload.prodId && {
//                     ...item,
//                     qty: action.payload.qty,
//                   },
//               ),
//             },
//         ),
//       };
//     case "REMOVE_FROM_CART":
//       return {
//         ...state,
//         farmhubs: state.farmhubs.map((farmhub) =>
//           farmhub.prodItems.filter((item) => item.id !== action.payload),
//         ),
//       };
//     case "CLEAR_CART":
//       return (state = initialState);
//     default:
//       return state;
//   }
// };
//
// export default cartReducer;

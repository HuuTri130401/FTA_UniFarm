export const cartItems = [
  {
    name: "Australian Orange",
    price: "12.30",
    qty: 4,
    desc: "Sweet and juicy",
    shadow: "orange",
    image: require("../assets/images/orange.png"),
    color: (opacity) => `rgba(251, 216, 146, ${opacity})`,
  },
  {
    name: "Flesh Nectari",
    shadow: "red",
    price: "12",
    qty: 3,
    desc: "Sweet and juicy",
    image: require("../assets/images/peach.png"),
    color: (opacity) => `rgba(255, 170, 157, ${opacity})`,
  },

  {
    name: "Black Grapes",
    price: "40",
    qty: 2,
    desc: "Sweet and juicy",
    shadow: "purple",
    image: require("../assets/images/grapes.png"),
    color: (opacity) => `rgba(214, 195, 246, ${opacity})`,
  },
  {
    name: "Green Apple",
    price: "10.5",
    qty: 2,
    desc: "Sweet and juicy",
    shadow: "green",
    image: require("../assets/images/greenApple.png"),
    color: (opacity) => `rgba(139, 243, 139, ${opacity})`,
  },
];

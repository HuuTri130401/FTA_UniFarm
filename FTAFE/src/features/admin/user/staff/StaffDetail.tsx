export {};
// import { Staff } from '@models/staff';
// import { convertTextToAvatar } from '@utils/string.helper';
// import { Button, Descriptions, Image } from 'antd';
// import moment from 'moment';
// import * as React from 'react';

// import UpdateUserModal from '../components/UpdateUserModal';

// interface StaffDetailProps {
//     staff: Staff;
// }

// const StaffDetail: React.FunctionComponent<StaffDetailProps> = ({ staff }) => {
//     const [openUpdateModal, setOpenUpdateModal] = React.useState<boolean>(false);

//     return (
//         <>
//             <div className="flex flex-col w-full gap-4">
//                 <Descriptions
//                     labelStyle={{
//                         fontWeight: 'bold',
//                     }}
//                     bordered
//                     title={'Basic Information'}
//                     className="p-4 bg-white rounded-lg"
//                     extra={
//                         <Button type="primary" onClick={() => setOpenUpdateModal(!openUpdateModal)}>
//                             Update
//                         </Button>
//                     }
//                 >
//                     <Descriptions.Item label="Avatar" span={1}>
//                         <Image
//                             height={80}
//                             width={80}
//                             className="rounded overflow-hidden"
//                             src={staff.avatar ? staff.avatar : convertTextToAvatar(staff.fullName)}
//                             alt={staff.fullName}
//                         />
//                     </Descriptions.Item>
//                     <Descriptions.Item label="Fullname" span={2}>
//                         {staff.fullName}
//                     </Descriptions.Item>
//                     <Descriptions.Item label="Status" span={1}>
//                         {staff.status}
//                     </Descriptions.Item>
//                     <Descriptions.Item label="Created at" span={1}>
//                         {moment(staff.createdAt).format('DD/MM/YYYY HH:mm')}
//                     </Descriptions.Item>
//                     <Descriptions.Item label="Last Updated At" span={1}>
//                         {moment(staff.updatedAt).format('DD/MM/YYYY HH:mm')}
//                     </Descriptions.Item>
//                 </Descriptions>
//             </div>
//             <UpdateUserModal
//                 currentValue={{
//                     avatar: staff.avatar,
//                     fullName: staff.fullName,
//                     id: staff.id,
//                     phone: staff.phone,
//                 }}
//                 open={openUpdateModal}
//                 afterClose={() => setOpenUpdateModal(false)}
//             />
//         </>
//     );
// };

// export default StaffDetail;

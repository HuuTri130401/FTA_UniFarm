import { Carousel, Image, Modal, ModalProps } from 'antd';

interface CreateStaffModalProps extends ModalProps {
    imageList: string[];
}
const contentStyle: React.CSSProperties = {
    margin: 0,
    height: '160px',
    color: '#fff',
    lineHeight: '160px',
    textAlign: 'center',
    background: '#364d79',
};
const ImageCarouselModal: React.FC<CreateStaffModalProps> = ({ imageList, ...rest }) => {
    const onChange = (currentSlide: number) => {
        // console.log(currentSlide);
    };

    return (
        <Modal {...rest} width={1000} footer={null}>
            <Carousel afterChange={onChange}>
                <Carousel afterChange={onChange}>
                    {imageList.map((i, index) => (
                        <div key={index}>
                            <Image src={i} preview={false} />
                        </div>
                    ))}
                </Carousel>
            </Carousel>
        </Modal>
    );
};

export default ImageCarouselModal;

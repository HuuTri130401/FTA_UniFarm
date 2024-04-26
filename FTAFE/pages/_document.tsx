import Document, { DocumentContext, Head, Html, Main, NextScript } from 'next/document';

class MyDocument extends Document {
    static async getInitialProps(ctx: DocumentContext) {
        return Document.getInitialProps(ctx);
    }

    render() {
        return (
            <Html>
                <Head>
                    <meta charSet="utf-8" />
                    <meta httpEquiv="X-UA-Compatible" content="IE=edge" />
                    <link rel="apple-touch-icon" sizes="57x57" href="/assets/images/logo/logo-main.png" />
                    <link rel="apple-touch-icon" sizes="60x60" href="/assets/images/logo/logo-main.png" />
                    <link rel="apple-touch-icon" sizes="72x72" href="/assets/images/logo/logo-main.png" />
                    <link rel="apple-touch-icon" sizes="76x76" href="/assets/images/logo/logo-main.png" />
                    <link rel="apple-touch-icon" sizes="114x114" href="/assets/images/logo/logo-main.png" />
                    <link rel="apple-touch-icon" sizes="120x120" href="/assets/images/logo/logo-main.png" />
                    <link rel="apple-touch-icon" sizes="144x144" href="/assets/images/logo/logo-main.png" />
                    <link rel="apple-touch-icon" sizes="152x152" href="/assets/images/logo/logo-main.png" />
                    <link rel="apple-touch-icon" sizes="180x180" href="/assets/images/logo/logo-main.png" />
                    <link rel="icon" type="image/png" sizes="192x192" href="/assets/images/logo/logo-main.png" />
                    <link rel="icon" type="image/png" sizes="32x32" href="/assets/images/logo/logo-main.png" />
                    <link rel="icon" type="image/png" sizes="96x96" href="/assets/images/logo/logo-main.png" />
                    <link rel="icon" type="image/png" sizes="16x16" href="/assets/images/logo/logo-main.png" />
                    <link rel="manifest" href="/assets/favicon/manifest.json" />
                </Head>
                <body>
                    <Main />
                    <NextScript />
                </body>
            </Html>
        );
    }
}

export default MyDocument;

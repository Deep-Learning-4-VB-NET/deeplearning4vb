Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports WeightsFormat = DeepLearning4VB.org.nd4vb.enums.WeightsFormat
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.convolution.config
Imports Pooling2DT = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.convolution.Pooling2D
Imports Pooling3DT = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.convolution.Pooling3D

Namespace org.nd4vb.linalg.api.ops.impl.layers.convolution.config

    ''' <summary>
    ''' The convolution and pooling configurations: Lombok's builder with its <c>@Builder.Default</c> values, the
    ''' validation of the all-arguments constructors, the <c>toProperties</c> maps (key order and values) and the
    ''' Lombok <c>@Data</c> accessors, equality and <c>toString</c>.
    ''' </summary>
    <TestClass>
    Public Class ConvolutionConfigTests

        Private Shared Sub AssertProperties(ByVal props As IDictionary(Of String, Object), ByVal keys As String(), ByVal values As Object())
            assertEquals(keys.ToList(), props.Keys.ToList())
            For i As Integer = 0 To keys.Length - 1
                assertEquals(values(i), props(keys(i)), keys(i))
            Next
        End Sub

        <TestMethod>
        Public Sub Conv2DConfigBuilderDefaults()
            Dim c As Conv2DConfig = Conv2DConfig.builder().build()
            assertEquals(-1L, c.getKH())
            assertEquals(-1L, c.getKW())
            assertEquals(1L, c.getSH())
            assertEquals(1L, c.getSW())
            assertEquals(0L, c.getPH())
            assertEquals(0L, c.getPW())
            assertEquals(1L, c.getDH())
            assertEquals(1L, c.getDW())
            assertFalse(c.isSameMode())
            assertEquals("NCHW", c.getDataFormat())
            assertEquals(WeightsFormat.YXIO, c.getWeightsFormat())
            assertFalse(c.isNHWC())
            ' the parameterless constructor (Lombok's @NoArgsConstructor) keeps the same defaults
            assertEquals(c, New Conv2DConfig())
        End Sub

        <TestMethod>
        Public Sub Conv2DConfigPropertiesAndData()
            Dim c As Conv2DConfig = Conv2DConfig.builder().kH(3).kW(2).sH(2).pW(1).isSameMode(True).dataFormat("NHWC").build()
            AssertProperties(c.toProperties(), {"kH", "kW", "sH", "sW", "pH", "pW", "dH", "dW", "isSameMode", "dataFormat"},
                             New Object() {3L, 2L, 2L, 1L, 0L, 1L, 1L, 1L, True, "NHWC"})
            assertTrue(c.isNHWC())
            Dim same As Conv2DConfig = Conv2DConfig.builder().kH(3).kW(2).sH(2).pW(1).isSameMode(True).dataFormat("NHWC").build()
            assertEquals(c, same)
            assertEquals(c.GetHashCode(), same.GetHashCode())
            assertFalse(c.Equals(Conv2DConfig.builder().kH(3).build()))
            assertEquals("Conv2DConfig(kH=3, kW=2, sH=2, sW=1, pH=0, pW=1, dH=1, dW=1, isSameMode=true, dataFormat=NHWC, weightsFormat=YXIO)", c.ToString())
            c.isNHWC(False)
            assertEquals("NCHW", c.getDataFormat())
            c.setKH(5)
            assertEquals(5L, c.getKH())
            c.setSameMode(False)
            assertFalse(c.isSameMode())
        End Sub

        <TestMethod>
        Public Sub ConfigurationsAreValidated()
            ' strides >= 1, padding >= 0, dilation >= 1, kernel <> 0 (ConvConfigUtil)
            assertThrows(Of ArgumentException)(Sub() Conv2DConfig.builder().sH(0).build())
            assertThrows(Of ArgumentException)(Sub() Conv2DConfig.builder().pW(-1).build())
            assertThrows(Of ArgumentException)(Sub() Conv2DConfig.builder().kH(0).build())
            assertThrows(Of ArgumentException)(Sub() Conv2DConfig.builder().dataFormat(Nothing).build())
            assertThrows(Of ArgumentException)(Sub() Conv1DConfig.builder().d(0).build())
            assertThrows(Of ArgumentException)(Sub() Conv3DConfig.builder().sD(0).build())
            assertThrows(Of ArgumentException)(Sub() DeConv2DConfig.builder().dW(0).build())
            assertThrows(Of ArgumentException)(Sub() DeConv3DConfig.builder().pD(-1).build())
            assertThrows(Of ArgumentException)(Sub() Pooling2DConfig.builder().sW(0).build())
            assertThrows(Of ArgumentException)(Sub() Pooling3DConfig.builder().kD(0).build())
            assertThrows(Of ArgumentException)(Sub() LocalResponseNormalizationConfig.builder().depth(0).build())
            ' an unknown data format is caught by the isX() getters
            assertThrows(Of InvalidOperationException)(Sub() Conv2DConfig.builder().dataFormat("NWHC").build().isNHWC())
        End Sub

        <TestMethod>
        Public Sub Conv1DConfigDefaultsAndProperties()
            Dim c As Conv1DConfig = Conv1DConfig.builder().k(3).build()
            assertEquals(3L, c.getK())
            assertEquals(1L, c.getS())
            assertEquals(0L, c.getP())
            assertEquals(1L, c.getD())
            assertEquals(Conv1DConfig.NCW, c.getDataFormat())
            ' Java leaves an unset padding mode null (the constructor then rejects it); the VB enum defaults to VALID
            assertEquals(PaddingMode.VALID, c.getPaddingMode())
            assertFalse(c.isNWC())
            Dim s As Conv1DConfig = Conv1DConfig.builder().k(2).paddingMode(PaddingMode.SAME).dataFormat(Conv1DConfig.NWC).build()
            AssertProperties(s.toProperties(), {"k", "s", "p", "d", "paddingMode", "dataFormat"},
                             New Object() {2L, 1L, 0L, 1L, PaddingMode.SAME, "NWC"})
            assertTrue(s.isNWC())
            ' the Java constructor taking isSameMode
            assertEquals(PaddingMode.SAME, New Conv1DConfig(2, 1, 0, Conv1DConfig.NCW, True).getPaddingMode())
        End Sub

        <TestMethod>
        Public Sub Conv3DAndDeconvolutionConfigs()
            Dim c3 As Conv3DConfig = Conv3DConfig.builder().kD(2).kH(3).kW(4).build()
            AssertProperties(c3.toProperties(), {"kD", "kW", "kH", "sD", "sW", "sH", "pD", "pW", "pH", "dD", "dW", "dH", "biasUsed", "dataFormat", "isSameMode"},
                             New Object() {2L, 4L, 3L, 1L, 1L, 1L, 0L, 0L, 0L, 1L, 1L, 1L, False, "NDHWC", False})
            assertFalse(c3.isNCDHW())
            c3.isNCDHW(True)
            assertEquals(Conv3DConfig.NCDHW, c3.getDataFormat())
            assertTrue(c3.isNCDHW())

            Dim d2 As DeConv2DConfig = DeConv2DConfig.builder().kH(2).kW(2).sH(2).sW(2).build()
            AssertProperties(d2.toProperties(), {"kH", "kW", "sH", "sW", "pH", "pW", "dH", "dW", "isSameMode", "dataFormat"},
                             New Object() {2L, 2L, 2L, 2L, 0L, 0L, 1L, 1L, False, DeConv2DConfig.NCHW})
            Dim d3 As DeConv3DConfig = DeConv3DConfig.builder().kD(1).kH(1).kW(1).isSameMode(True).build()
            AssertProperties(d3.toProperties(), {"kD", "kH", "kW", "sD", "sH", "sW", "pD", "pH", "pW", "dD", "dH", "dW", "isSameMode", "dataFormat"},
                             New Object() {1L, 1L, 1L, 1L, 1L, 1L, 0L, 0L, 0L, 1L, 1L, 1L, True, DeConv3DConfig.NCDHW})
        End Sub

        <TestMethod>
        Public Sub PoolingConfigs()
            Dim p2 As Pooling2DConfig = Pooling2DConfig.builder().kH(2).kW(3).build()
            assertEquals(Pooling2DT.Pooling2DType.MAX, p2.getType())
            assertEquals(Pooling2DT.Divisor.EXCLUDE_PADDING, p2.getDivisor())
            assertFalse(p2.isNHWC())
            AssertProperties(p2.toProperties(), {"kH", "kW", "sH", "sW", "pH", "pW", "extra", "type", "isSameMode", "dH", "dW", "isNHWC"},
                             New Object() {2L, 3L, 1L, 1L, 0L, 0L, 0.0, "MAX", False, 1L, 1L, False})
            p2.setType(Pooling2DT.Pooling2DType.PNORM)
            p2.setExtra(2.0)
            assertEquals("PNORM", p2.toProperties()("type"))
            ' getType() is Lombok's getter; Object.GetType() is still reachable through CObj
            assertTrue(CObj(p2).GetType() Is GetType(Pooling2DConfig))

            Dim p3 As Pooling3DConfig = Pooling3DConfig.builder().kD(2).kW(2).kH(2).build()
            assertEquals(Pooling3DT.Pooling3DType.MAX, p3.getType())
            assertTrue(p3.isNCDHW())
            AssertProperties(p3.toProperties(), {"kD", "kW", "kH", "sD", "sW", "sH", "pD", "pW", "pH", "dD", "dW", "dH", "type", "isSameMode"},
                             New Object() {2L, 2L, 2L, 1L, 1L, 1L, 0L, 0L, 0L, 1L, 1L, 1L, "MAX", False})
        End Sub

        <TestMethod>
        Public Sub LocalResponseNormalizationConfigProperties()
            Dim c As LocalResponseNormalizationConfig = LocalResponseNormalizationConfig.builder().alpha(1.0E-4).beta(0.75).bias(1.0).depth(5).build()
            AssertProperties(c.toProperties(), {"alpha", "beta", "bias", "depth"}, New Object() {1.0E-4, 0.75, 1.0, 5})
            assertEquals("LocalResponseNormalizationConfig(alpha=1.0E-4, beta=0.75, bias=1.0, depth=5)", c.ToString())
            assertEquals(c, New LocalResponseNormalizationConfig(1.0E-4, 0.75, 1.0, 5))
        End Sub

    End Class

End Namespace

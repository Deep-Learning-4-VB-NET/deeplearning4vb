Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.recurrent.config
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.recurrent.weights

Namespace org.nd4vb.linalg.api.ops.impl.layers.recurrent.config

    ''' <summary>
    ''' The recurrent layer configurations and weights: Lombok builders and defaults, the property maps the ops report,
    ''' and the array arguments the weights contribute.
    ''' </summary>
    <TestClass>
    Public Class RecurrentConfigTests

        <TestMethod>
        Public Sub LstmLayerConfigDefaultsAndProperties()
            Dim c As LSTMLayerConfig = LSTMLayerConfig.builder().build()
            assertEquals(LSTMDataFormat.TNS, c.getLstmdataformat())
            assertEquals(LSTMDirectionMode.FWD, c.getDirectionMode())
            assertEquals(LSTMActivations.SIGMOID, c.getGateAct())
            assertEquals(LSTMActivations.TANH, c.getCellAct())
            assertEquals(LSTMActivations.TANH, c.getOutAct())
            assertTrue(c.isRetFullSequence())
            assertFalse(c.isRetLastH())
            assertFalse(c.isRetLastC())
            assertEquals(0.0, c.getCellClip(), 0.0)
            assertEquals(c, New LSTMLayerConfig())
            Dim props As IDictionary(Of String, Object) = c.toProperties(True, True)
            assertEquals(New List(Of String) From {"gateAct", "outAct", "cellAct", "retFullSequence", "retLastH", "retLastC", "cellClip", "lstmdataformat", "directionMode"},
                         props.Keys.ToList())
            assertEquals("SIGMOID", props("gateAct"))
            assertEquals("TNS", props("lstmdataformat"))
            assertEquals(7, c.toProperties(False, False).Count)
            ' Lombok's all-arguments constructor, in field order
            Dim all As New LSTMLayerConfig(LSTMDataFormat.NTS, LSTMDirectionMode.BWD, LSTMActivations.RELU, LSTMActivations.ELU, LSTMActivations.SOFTSIGN, False, True, True, 1.5)
            assertEquals(LSTMActivations.ELU, all.getCellAct())
            assertEquals(LSTMActivations.SOFTSIGN, all.getOutAct())
            assertEquals(1.5, all.getCellClip(), 0.0)
        End Sub

        <TestMethod>
        Public Sub LstmAndCellConfigurations()
            Dim c As LSTMConfiguration = LSTMConfiguration.builder().peepHole(True).forgetBias(1.0).clippingCellValue(0.5).build()
            assertEquals(RnnDataFormat.TNS, c.getDataFormat())
            Dim props As IDictionary(Of String, Object) = c.toProperties(True)
            assertEquals(New List(Of String) From {"peepHole", "clippingCellValue", "forgetBias", "dataFormat"}, props.Keys.ToList())
            assertEquals(True, props("peepHole"))
            assertEquals(0.5, props("clippingCellValue"))
            ' iArgs: peephole flag and data format ordinal; tArgs: forget bias, cell clipping
            assertArrayEquals(New Integer() {1, CInt(RnnDataFormat.TNS)}, c.iArgs(True))
            assertArrayEquals(New Double() {1.0, 0.5}, c.tArgs(), 0.0)

            Dim cell As LSTMCellConfiguration = LSTMCellConfiguration.builder().projection(True).clippingProjValue(2.0).build()
            assertEquals(New List(Of String) From {"peepHole", "projection", "clippingCellValue", "clippingProjValue", "forgetBias"},
                         cell.toProperties().Keys.ToList())
            assertEquals(2.0, cell.toProperties()("clippingProjValue"))
        End Sub

        <TestMethod>
        Public Sub WeightsContributeTheirArraysInJavaOrder()
            Dim w As INDArray = FakeArray.Simple()
            Dim rw As INDArray = FakeArray.Simple()
            Dim b As INDArray = FakeArray.Simple()
            Dim x As INDArray = FakeArray.Simple()
            Dim lw As LSTMLayerWeights = LSTMLayerWeights.builder().iWeights(w).irWeights(rw).iBias(b).build()
            assertTrue(lw.hasBias())
            assertFalse(lw.hasPH())
            Dim args As INDArray() = lw.argsWithInputs(x, Nothing, Nothing, Nothing)
            assertEquals(4, args.Length)
            assertTrue(args(0) Is x)
            assertTrue(args(1) Is w)
            assertTrue(args(2) Is rw)
            assertTrue(args(3) Is b)
            assertTrue(lw.getIWeights() Is w)

            Dim gru As GRUWeights = GRUWeights.builder().iRuWeights(w).iCWeight(rw).build()
            assertEquals(2, gru.arrayArgs().Length)
            assertEquals(gru.argsWithInputs(x).Length, 3)
            Dim sru As SRUWeights = SRUWeights.builder().iWeights(w).iBias(b).build()
            assertEquals(2, sru.arrayArgs().Length)
        End Sub

    End Class

End Namespace

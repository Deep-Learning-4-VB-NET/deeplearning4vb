Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.enums

'
' *  ******************************************************************************
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Namespace org.nd4vb.enums

    ''' <summary>
    ''' Pins the values of the generated op-argument enums: the ops pass them to the kernels as integer arguments (Java
    ''' ordinals), so their order must not change.
    ''' </summary>
    <TestClass>
    Public Class EnumsTests

        Private Shared Sub assertOrder(ByVal type As Type, ParamArray ByVal names() As String)
            Dim values As Array = [Enum].GetValues(type)
            assertEquals(names.Length, values.Length, type.Name)
            For i As Integer = 0 To names.Length - 1
                assertEquals(names(i), [Enum].GetName(type, i), type.Name)
            Next i
        End Sub

        <TestMethod>
        Public Sub testOrdinalsAreJavas()
            Dim acts As String() = {"TANH", "RELU", "SIGMOID", "AFFINE", "LEAKY_RELU", "THRESHHOLD_RELU", "SCALED_TAHN", "HARD_SIGMOID", "ELU", "SOFTSIGN", "SOFTPLUS"}
            assertOrder(GetType(CellAct), acts)
            assertOrder(GetType(GateAct), acts)
            assertOrder(GetType(OutAct), acts)
            assertOrder(GetType(DataFormat), "NCHW", "NHWC")
            assertOrder(GetType(ImageResizeMethod), "ResizeBilinear", "ResizeBicubic", "ResizeNearest", "ResizeGaussian", "ResizeLanczos5", "ResizeMitchelcubic", "ResizeArea")
            assertOrder(GetType(LSTMDataFormat), "TNS", "NST", "NTS", "T2NS")
            assertOrder(GetType(LSTMDirectionMode), "FWD", "BWD", "BIDIR_SUM", "BIDIR_CONCAT", "BIDIR_EXTRA_DIM")
            assertOrder(GetType(PadMode), "CONSTANT", "REFLECT", "SYMMETRIC")
            assertOrder(GetType(PartitionMode), "MOD", "DIV")
            assertOrder(GetType(RnnDataFormat), "TNS", "NST", "NTS")
            assertOrder(GetType(WeightsFormat), "YXIO", "OIYX", "OYXI")
        End Sub

    End Class

End Namespace

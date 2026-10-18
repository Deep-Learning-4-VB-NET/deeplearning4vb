Imports System
Imports LSTMDataFormat = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.recurrent.config.LSTMDataFormat
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports LSTMDirectionMode = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.recurrent.config.LSTMDirectionMode
Imports LSTMLayerConfig = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.recurrent.config.LSTMLayerConfig
#End If

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.nd4vb.linalg.api.ops.impl.layers.recurrent.outputs

    ''' <summary>
    ''' The outputs of a LSTM layer (<seealso cref="LSTMLayer"/>.
    ''' </summary>
    Public Class LSTMLayerOutputs

        ''' <summary>
        ''' The LSTM layer data format (<seealso cref="LSTMDataFormat"/>.
        ''' </summary>
        Private dataFormat As LSTMDataFormat

#If SAMEDIFF Then
		''' <summary>
		''' output h:
		''' [sL, bS, nOut]    when directionMode <= 2 && dataFormat == 0
		''' [bS, sL, nOut]    when directionMode <= 2 && dataFormat == 1
		''' [bS, nOut, sL]    when directionMode <= 2 && dataFormat == 2
		''' [sL, bS, 2*nOut]  when directionMode == 3 && dataFormat == 0
		''' [bS, sL, 2*nOut]  when directionMode == 3 && dataFormat == 1
		''' [bS, 2*nOut, sL]  when directionMode == 3 && dataFormat == 2
		''' [sL, 2, bS, nOut] when directionMode == 4 && dataFormat == 3
		''' numbers mean index in corresponding enums <seealso cref="LSTMDataFormat"/> and <seealso cref="LSTMDirectionMode"/>
		''' </summary>
		Private timeSeriesOutput As SDVariable

		''' <summary>
		''' cell state at last step cL:
		''' [bS, nOut]   when directionMode FWD or BWD
		''' 2, bS, nOut] when directionMode  BIDIR_SUM, BIDIR_CONCAT or BIDIR_EXTRA_DIM
		''' </summary>
		Private lastCellStateOutput As SDVariable

		''' <summary>
		''' output at last step hL:
		''' [bS, nOut]   when directionMode FWD or BWD
		''' 2, bS, nOut] when directionMode  BIDIR_SUM, BIDIR_CONCAT or BIDIR_EXTRA_DIM
		''' </summary>
		Private lastTimeStepOutput As SDVariable

		Public Sub New(ByVal outputs() As SDVariable, ByVal lstmLayerConfig As LSTMLayerConfig)
			Preconditions.checkArgument(outputs.Length > 0 AndAlso outputs.Length <= 3, "Must have from 1 to 3 LSTM layer outputs, got %s", outputs.Length)

			Dim i As Integer = 0
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: timeSeriesOutput = lstmLayerConfig.isRetFullSequence() ? outputs[i++] : null;
			timeSeriesOutput = If(lstmLayerConfig.isRetFullSequence(), outputs(i), Nothing)
				i += 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: lastTimeStepOutput = lstmLayerConfig.isRetLastH() ? outputs[i++] : null;
			lastTimeStepOutput = If(lstmLayerConfig.isRetLastH(), outputs(i), Nothing)
				i += 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: lastCellStateOutput = lstmLayerConfig.isRetLastC() ? outputs[i++] : null;
			lastCellStateOutput = If(lstmLayerConfig.isRetLastC(), outputs(i), Nothing)
				i += 1

			Me.dataFormat = lstmLayerConfig.getLstmdataformat()
		End Sub

		''' <summary>
		''' Get h, the output of the cell for all time steps.
		''' <para>
		''' Shape depends on data format defined in <seealso cref="LSTMLayerConfig "/>:<br>
		''' for unidirectional:
		''' TNS: shape [timeLength, numExamples, inOutSize] - sometimes referred to as "time major"<br>
		''' NST: shape [numExamples, inOutSize, timeLength]<br>
		''' NTS: shape [numExamples, timeLength, inOutSize] <br>
		''' for bidirectional:
		''' T2NS: 3 = [timeLength, 2, numExamples, inOutSize] (for ONNX)
		''' </para>
		''' </summary>
		Public Overridable ReadOnly Property Output As SDVariable
			Get
				Preconditions.checkArgument(timeSeriesOutput IsNot Nothing, "retFullSequence was setted as false in LSTMLayerConfig")
				Return timeSeriesOutput
			End Get
		End Property

		Public Overridable ReadOnly Property LastState As SDVariable
			Get
				Preconditions.checkArgument(lastCellStateOutput IsNot Nothing, "retLastC was setted as false in LSTMLayerConfig")
				Return lastCellStateOutput
			End Get
		End Property

		Public Overridable ReadOnly Property LastOutput As SDVariable
			Get
				Preconditions.checkArgument(lastTimeStepOutput IsNot Nothing, "retLastH was setted as false in LSTMLayerConfig")
				Return lastTimeStepOutput
			End Get
		End Property
#End If

        ' ---------------------------------------------------------------- Lombok members

        ''' <summary>Lombok getter of <c>dataFormat</c>.</summary>
        Public Overridable Function getDataFormat() As LSTMDataFormat
            Return dataFormat
        End Function

#If SAMEDIFF Then
        ''' <summary>Lombok getter of <c>timeSeriesOutput</c>.</summary>
        Public Overridable Function getTimeSeriesOutput() As SDVariable
            Return timeSeriesOutput
        End Function

        ''' <summary>Lombok getter of <c>lastCellStateOutput</c>.</summary>
        Public Overridable Function getLastCellStateOutput() As SDVariable
            Return lastCellStateOutput
        End Function

        ''' <summary>Lombok getter of <c>lastTimeStepOutput</c>.</summary>
        Public Overridable Function getLastTimeStepOutput() As SDVariable
            Return lastTimeStepOutput
        End Function
#End If

    End Class

End Namespace

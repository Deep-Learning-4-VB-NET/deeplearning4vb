Imports System.Collections.Generic
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports LSTMBlockCell = org.nd4j.linalg.api.ops.impl.layers.recurrent.LSTMBlockCell
Imports LSTMLayer = org.nd4j.linalg.api.ops.impl.layers.recurrent.LSTMLayer
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil

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

Namespace org.nd4j.linalg.api.ops.impl.layers.recurrent.config


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder @Data public class LSTMConfiguration
	Public Class LSTMConfiguration

		''' <summary>
		''' Whether to provide peephole connections.
		''' </summary>
		Private peepHole As Boolean 'IArg(0)

		''' <summary>
		''' The data format of the input.  Only used in <seealso cref="LSTMLayer"/>, ignored in <seealso cref="LSTMBlockCell"/>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private RnnDataFormat dataFormat = RnnDataFormat.TNS;
		Private dataFormat As RnnDataFormat = RnnDataFormat.TNS 'IArg(1) (only for lstmBlock, not lstmBlockCell)

		''' <summary>
		''' The bias added to forget gates in order to reduce the scale of forgetting in the beginning of the training.
		''' </summary>
		Private forgetBias As Double 'TArg(0)

		''' <summary>
		''' Clipping value for cell state, if it is not equal to zero, then cell state is clipped.
		''' </summary>
		Private clippingCellValue As Double 'TArg(1)

		Public Overridable Function toProperties(ByVal includeDataFormat As Boolean) As IDictionary(Of String, Object)
			Dim ret As IDictionary(Of String, Object) = New LinkedHashMap(Of String, Object)()
			ret("peepHole") = peepHole
			ret("clippingCellValue") = clippingCellValue
			ret("forgetBias") = forgetBias
			If includeDataFormat Then
				ret("dataFormat") = dataFormat
			End If
			Return ret
		End Function


		Public Overridable Function iArgs(ByVal includeDataFormat As Boolean) As Integer()
			If includeDataFormat Then
				Return New Integer(){ArrayUtil.fromBoolean(peepHole), dataFormat.ordinal()}
			Else
				Return New Integer(){ArrayUtil.fromBoolean(peepHole)}
			End If
		End Function

		Public Overridable Function tArgs() As Double()
			Return New Double() {forgetBias, clippingCellValue}
		End Function
	End Class

End Namespace
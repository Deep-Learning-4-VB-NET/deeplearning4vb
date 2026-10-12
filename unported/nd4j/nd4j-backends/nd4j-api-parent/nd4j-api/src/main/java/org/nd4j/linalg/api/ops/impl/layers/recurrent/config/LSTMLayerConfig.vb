Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor

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
'ORIGINAL LINE: @Builder @Data @AllArgsConstructor @NoArgsConstructor public class LSTMLayerConfig
	Public Class LSTMLayerConfig
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private LSTMDataFormat lstmdataformat = LSTMDataFormat.TNS;
		Private lstmdataformat As LSTMDataFormat = LSTMDataFormat.TNS 'INT_ARG(0)


		''' <summary>
		''' direction <br>
		''' FWD: 0 = fwd
		''' BWD: 1 = bwd
		''' BS: 2 = bidirectional sum
		''' BC: 3 = bidirectional concat
		''' BE: 4 = bidirectional extra output dim (in conjunction with format dataFormat = 3)
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private LSTMDirectionMode directionMode = LSTMDirectionMode.FWD;
		Private directionMode As LSTMDirectionMode = LSTMDirectionMode.FWD 'INT_ARG(1)

		''' <summary>
		''' Activation for input (i), forget (f) and output (o) gates
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private LSTMActivations gateAct = LSTMActivations.SIGMOID;
		Private gateAct As LSTMActivations = LSTMActivations.SIGMOID ' INT_ARG(2)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private LSTMActivations cellAct = LSTMActivations.TANH;
		Private cellAct As LSTMActivations = LSTMActivations.TANH ' INT_ARG(3)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private LSTMActivations outAct = LSTMActivations.TANH;
		Private outAct As LSTMActivations = LSTMActivations.TANH ' INT_ARG(4)




		''' <summary>
		''' indicates whether to return whole time sequence h {h_0, h_1, ... , h_sL-1}
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private boolean retFullSequence = true;
		Private retFullSequence As Boolean = True 'B_ARG(5)

		''' <summary>
		''' indicates whether to return output at last time step only,
		''' in this case shape would be [bS, nOut] (exact shape depends on dataFormat argument)
		''' </summary>
		Private retLastH As Boolean 'B_ARG(6)

		''' <summary>
		''' indicates whether to return cells state at last time step only,
		''' in this case shape would be [bS, nOut] (exact shape depends on dataFormat argument)
		''' </summary>
		Private retLastC As Boolean ' B_ARG(7)

		''' <summary>
		''' Cell clipping value, if it = 0 then do not apply clipping
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private double cellClip = 0;
		Private cellClip As Double = 0 'T_ARG(0)


		Public Overridable Function toProperties(ByVal includeLSTMDataFormat As Boolean, ByVal includeLSTMDirectionMode As Boolean) As IDictionary(Of String, Object)
			Dim ret As IDictionary(Of String, Object) = New LinkedHashMap(Of String, Object)()
			ret("gateAct") = gateAct.ToString()
			ret("outAct") = outAct.ToString()
			ret("cellAct") = cellAct.ToString()
			ret("retFullSequence") = retFullSequence
			ret("retLastH") = retLastH
			ret("retLastC") = retLastC
			ret("cellClip") = cellClip

			If includeLSTMDataFormat Then
				ret("lstmdataformat") = lstmdataformat.ToString()
			End If
			If includeLSTMDirectionMode Then
				ret("directionMode") = directionMode.ToString()
			End If
			Return ret
		End Function

	End Class







End Namespace
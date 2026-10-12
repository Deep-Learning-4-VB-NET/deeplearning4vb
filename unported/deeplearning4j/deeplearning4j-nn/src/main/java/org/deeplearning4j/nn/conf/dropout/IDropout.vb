Imports System
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports JsonTypeInfo = org.nd4j.shade.jackson.annotation.JsonTypeInfo

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

Namespace org.deeplearning4j.nn.conf.dropout


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "@class") public interface IDropout extends java.io.Serializable, Cloneable
	Public Interface IDropout
		Inherits ICloneable

		''' <param name="inputActivations"> Input activations array </param>
		''' <param name="resultArray">      The result array (same as inputArray for in-place ops) for the post-dropout activations </param>
		''' <param name="iteration">        Current iteration number </param>
		''' <param name="epoch">            Current epoch number </param>
		''' <param name="workspaceMgr">     Workspace manager, if any storage is required (use ArrayType.INPUT) </param>
		''' <returns> The output (resultArray) after applying dropout </returns>
		Function applyDropout(ByVal inputActivations As INDArray, ByVal resultArray As INDArray, ByVal iteration As Integer, ByVal epoch As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray

		''' <summary>
		''' Perform backprop. This should also clear the internal state (dropout mask) if any is present
		''' </summary>
		''' <param name="gradAtOutput"> Gradients at the output of the dropout op - i.e., dL/dOut </param>
		''' <param name="gradAtInput">  Gradients at the input of the dropout op - i.e., dL/dIn. Use the same array as gradAtOutput
		'''                     to apply the backprop gradient in-place </param>
		''' <param name="iteration">    Current iteration </param>
		''' <param name="epoch">        Current epoch </param>
		''' <returns> Same array as gradAtInput - i.e., gradient after backpropagating through dropout op - i.e., dL/dIn </returns>
		Function backprop(ByVal gradAtOutput As INDArray, ByVal gradAtInput As INDArray, ByVal iteration As Integer, ByVal epoch As Integer) As INDArray

		''' <summary>
		''' Clear the internal state (for example, dropout mask) if any is present
		''' </summary>
		Sub clear()

		Function clone() As IDropout
	End Interface

End Namespace
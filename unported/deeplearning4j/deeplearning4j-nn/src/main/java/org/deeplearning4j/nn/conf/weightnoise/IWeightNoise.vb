Imports System
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
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

Namespace org.deeplearning4j.nn.conf.weightnoise


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "@class") public interface IWeightNoise extends java.io.Serializable, Cloneable
	Public Interface IWeightNoise
		Inherits ICloneable

		''' <summary>
		''' Get the parameter, after applying weight noise
		''' </summary>
		''' <param name="layer">     Layer to get the parameter for </param>
		''' <param name="paramKey">  Parameter key </param>
		''' <param name="iteration"> Iteration number </param>
		''' <param name="epoch">     Epoch number </param>
		''' <param name="train">     If true: training. False: at test time </param>
		''' <returns>          Parameter, after applying weight noise </returns>
		Function getParameter(ByVal layer As Layer, ByVal paramKey As String, ByVal iteration As Integer, ByVal epoch As Integer, ByVal train As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray

		Function clone() As IWeightNoise

	End Interface

End Namespace
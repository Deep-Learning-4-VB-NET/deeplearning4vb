Imports System
Imports System.Collections.Generic
Imports Layer = org.deeplearning4j.nn.api.Layer
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

Namespace org.deeplearning4j.nn.api.layers


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "@class") public interface LayerConstraint extends Cloneable, java.io.Serializable
	Public Interface LayerConstraint
		Inherits ICloneable

		''' <summary>
		''' Apply a given constraint to a layer at each iteration
		''' in the provided epoch, after parameters have been updated.
		''' </summary>
		''' <param name="layer"> org.deeplearning4j.nn.api.Layer </param>
		''' <param name="iteration"> given iteration as integer </param>
		''' <param name="epoch"> current epoch as integer </param>
		Sub applyConstraint(ByVal layer As Layer, ByVal iteration As Integer, ByVal epoch As Integer)

		''' <summary>
		''' Set the parameters that this layer constraint should be applied to
		''' </summary>
		''' <param name="params"> Parameters that the layer constraint should be applied to </param>
		Property Params As ISet(Of String)


		Function clone() As LayerConstraint

	End Interface

End Namespace
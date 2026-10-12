Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports JsonAutoDetect = org.nd4j.shade.jackson.annotation.JsonAutoDetect
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

Namespace org.deeplearning4j.nn.weights


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "@class") @JsonAutoDetect(fieldVisibility = JsonAutoDetect.Visibility.ANY, getterVisibility = JsonAutoDetect.Visibility.NONE, setterVisibility = JsonAutoDetect.Visibility.NONE) public interface IWeightInit extends java.io.Serializable
	Public Interface IWeightInit

		' Use this in a default method when java 8 support is added
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in VB:
'		char DEFAULT_WEIGHT_INIT_ORDER = WeightInitUtil.DEFAULT_WEIGHT_INIT_ORDER;

		''' <summary>
		''' Initialize parameters in the given view. Double values are used for fanIn and fanOut as some layers
		''' (convolution with stride) results in a non-integer number which may be truncated to zero in certain configurations </summary>
		''' <param name="fanIn"> Number of input parameters </param>
		''' <param name="fanOut"> Number of output parameters </param>
		''' <param name="shape"> Desired shape of array (users shall assume paramView has this shape after method has finished) </param>
		''' <param name="order"> Order of array, e.g. Fortran ('f') or C ('c') </param>
		''' <param name="paramView"> View of parameters to initialize (and reshape) </param>
		Function init(ByVal fanIn As Double, ByVal fanOut As Double, ByVal shape() As Long, ByVal order As Char, ByVal paramView As INDArray) As INDArray
	End Interface

End Namespace
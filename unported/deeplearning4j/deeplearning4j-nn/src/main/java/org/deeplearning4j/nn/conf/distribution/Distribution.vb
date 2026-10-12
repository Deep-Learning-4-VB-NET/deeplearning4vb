Imports System
Imports LegacyDistributionHelper = org.deeplearning4j.nn.conf.distribution.serde.LegacyDistributionHelper
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

Namespace org.deeplearning4j.nn.conf.distribution


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "type", defaultImpl = LegacyDistributionHelper.class) public abstract class Distribution implements java.io.Serializable, Cloneable
	<Serializable>
	Public MustInherit Class Distribution
		Implements ICloneable

		Private Const serialVersionUID As Long = 5401741214954998498L

		Public Overrides Function clone() As Distribution
			Try
				Return CType(MyBase.clone(), Distribution)
			Catch e As CloneNotSupportedException
				Throw New Exception(e)
			End Try
		End Function
	End Class

End Namespace
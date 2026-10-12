Imports System
Imports DoubleQuality = org.datavec.api.transform.quality.columns.DoubleQuality
Imports org.nd4j.common.function

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

Namespace org.datavec.api.transform.analysis.quality.real


	<Serializable>
	Public Class RealQualityMergeFunction
		Implements BiFunction(Of DoubleQuality, DoubleQuality, DoubleQuality)

		Public Overridable Function apply(ByVal v1 As DoubleQuality, ByVal v2 As DoubleQuality) As DoubleQuality
			Return v1.add(v2)
		End Function
	End Class

End Namespace
Imports System.Collections.Generic

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

Namespace org.nd4j.common.util


	Public Interface MultiValueMap(Of K, V)
		Inherits IDictionary(Of K, IList(Of V))

		Function getFirst(ByVal var1 As K) As V

		Sub add(ByVal var1 As K, ByVal var2 As V)

		Sub set(ByVal var1 As K, ByVal var2 As V)

		WriteOnly Property All As IDictionary(Of K, V)

		Function toSingleValueMap() As IDictionary(Of K, V)
	End Interface

End Namespace
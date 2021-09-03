Imports DataSet = org.nd4j.linalg.dataset.DataSet

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

Namespace org.nd4j.linalg.dataset.api.iterator.cache

	Public Interface DataSetCache
		''' <summary>
		''' Check is given namespace has complete cache of the data set </summary>
		''' <param name="namespace"> </param>
		''' <returns> true if namespace is fully cached </returns>
		Function isComplete(ByVal [namespace] As String) As Boolean

		''' <summary>
		''' Sets the flag indicating whether given namespace is fully cached </summary>
		''' <param name="namespace"> </param>
		''' <param name="value"> </param>
		Sub setComplete(ByVal [namespace] As String, ByVal value As Boolean)

		Function get(ByVal key As String) As DataSet

		Sub put(ByVal key As String, ByVal dataSet As DataSet)

		Function contains(ByVal key As String) As Boolean
	End Interface

End Namespace
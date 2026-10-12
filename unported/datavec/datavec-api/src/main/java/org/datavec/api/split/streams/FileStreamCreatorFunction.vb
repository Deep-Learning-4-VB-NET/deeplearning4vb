Imports System
Imports System.IO
Imports Preconditions = org.nd4j.common.base.Preconditions
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

Namespace org.datavec.api.split.streams


	<Serializable>
	Public Class FileStreamCreatorFunction
		Implements [Function](Of URI, Stream)

		Public Overridable Function apply(ByVal uri As URI) As Stream
			Preconditions.checkState(uri.getScheme() Is Nothing OrElse uri.getScheme().equalsIgnoreCase("file"), "Attempting to open URI that is not a File URI; for other stream types, you must use an appropriate stream loader function. URI: %s", uri)
			Try
				Return New FileStream(uri, FileMode.Open, FileAccess.Read)
			Catch e As IOException
				Throw New Exception("Error loading stream for file: " & uri, e)
			End Try
		End Function

	End Class

End Namespace
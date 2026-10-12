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

Namespace org.deeplearning4j.spark.stats


	Public Interface EventStats

		ReadOnly Property MachineID As String

		ReadOnly Property JvmID As String

		ReadOnly Property ThreadID As Long

		ReadOnly Property StartTime As Long

		ReadOnly Property DurationMs As Long

		''' <summary>
		''' Get a String representation of the EventStats. This should be a single line delimited representation, suitable
		''' for exporting (such as CSV). Should not contain a new-line character at the end of each line
		''' </summary>
		''' <param name="delimiter"> Delimiter to use for the data </param>
		''' <returns> String representation of the EventStats object </returns>
		Function asString(ByVal delimiter As String) As String

		''' <summary>
		''' Get a header line for exporting the EventStats object, for use with <seealso cref="asString(String)"/>
		''' </summary>
		''' <param name="delimiter"> Delimiter to use for the header </param>
		''' <returns> Header line </returns>
		Function getStringHeader(ByVal delimiter As String) As String
	End Interface

End Namespace
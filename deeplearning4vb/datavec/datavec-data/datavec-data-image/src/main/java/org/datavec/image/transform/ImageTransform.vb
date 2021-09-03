Imports System
Imports org.datavec.api.transform
Imports ImageWritable = org.datavec.image.data.ImageWritable
Imports JsonInclude = org.nd4j.shade.jackson.annotation.JsonInclude
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

Namespace org.datavec.image.transform


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonInclude(JsonInclude.Include.NON_NULL) @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "@class") public interface ImageTransform extends org.datavec.api.transform.Operation<org.datavec.image.data.ImageWritable, org.datavec.image.data.ImageWritable>
	Public Interface ImageTransform
		Inherits Operation(Of ImageWritable, ImageWritable)

		''' <summary>
		''' Takes an image and returns a transformed image.
		''' Uses the random object in the case of random transformations.
		''' </summary>
		''' <param name="image">  to transform, null == end of stream </param>
		''' <param name="random"> object to use (or null for deterministic) </param>
		''' <returns>       transformed image </returns>
		Function transform(ByVal image As ImageWritable, ByVal random As Random) As ImageWritable

		''' <summary>
		''' Transforms the given coordinates using the parameters that were used to transform the last image.
		''' </summary>
		''' <param name="coordinates"> to transforms (x1, y1, x2, y2, ...) </param>
		''' <returns>            transformed coordinates </returns>
		Function query(ParamArray ByVal coordinates() As Single) As Single()

		''' <summary>
		''' Returns the last transformed image or null if none transformed yet.
		''' </summary>
		''' <returns> Last transformed image or null </returns>
		ReadOnly Property CurrentImage As ImageWritable
	End Interface

End Namespace